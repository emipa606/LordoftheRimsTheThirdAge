using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace TheThirdAge
{
    public class CompMedievalRottable : CompRottable
    {
        public override string CompInspectStringExtra()
        {
            var sb = new StringBuilder();
            switch (Stage)
            {
                case RotStage.Fresh:
                    sb.AppendLine("RotStateFresh".Translate());
                    break;
                case RotStage.Rotting:
                    sb.AppendLine("RotStateRotting".Translate());
                    break;
                case RotStage.Dessicated:
                    sb.AppendLine("RotStateDessicated".Translate());
                    break;
            }

            var num = PropsRot.TicksToRotStart - RotProgress;
            if (!(num > 0f))
            {
                return sb.ToString().TrimEndNewlines();
            }

            var num2 = GenTemperature.GetTemperatureForCell(parent.PositionHeld, parent.Map);
            var thingList = parent.PositionHeld.GetThingList(parent.Map);
            var factor = 1f;
            foreach (var thing in thingList)
            {
                if (!(thing is Building_RottableFixer))
                {
                    continue;
                }

                var b = thing as Building_RottableFixer;
                var isMeat = parent?.def.IsMeat ?? false;
                if (!isMeat && b.def.defName == "LotR_PantryShelf")
                {
                    factor = 3f;
                }

                if (b.def.defName == "LotR_SaltBarrel")
                {
                    factor = 15f;
                }

                //num2 = building_Refrigerator.CurrentTemp;
                break;
            }

            num2 = Mathf.RoundToInt(num2);
            var num3 = GenTemperature.RotRateAtTemperature(num2);
            var ticksUntilRotAtCurrentTemp = (int) (TicksUntilRotAtCurrentTemp * factor);
            if (num3 < 0.001f)
            {
                sb.Append("CurrentlyFrozen".Translate() + ".");
            }
            else
            {
                if (num3 < 0.999f)
                {
                    sb.Append("CurrentlyRefrigerated".Translate(new object[]
                    {
                        ticksUntilRotAtCurrentTemp.ToStringTicksToPeriodVague()
                    }) + ".");
                }
                else
                {
                    sb.Append("NotRefrigerated".Translate(new object[]
                    {
                        ticksUntilRotAtCurrentTemp.ToStringTicksToPeriodVague()
                    }) + ".");
                }
            }

            return sb.ToString().TrimEndNewlines();
        }

        public override void CompTickRare()
        {
            if (parent.MapHeld == null || parent.Map == null)
            {
                return;
            }

            var list = new HashSet<Thing>(parent.MapHeld.thingGrid.ThingsListAtFast(parent.PositionHeld));
            var isMeat = parent?.def.IsMeat ?? false;
            var isSalted = parent?.def?.defName?.ToLowerInvariant().Contains("salted") ?? false;
            var saltPot =
                list.FirstOrDefault(x => x is Building_RottableFixer && x.def.defName == "LotR_SaltBarrel");
            if (isMeat && !isSalted && saltPot != null)
            {
                HandleMeatThatNeedsSalting(parent);
                return;
            }

            var rotProgress = RotProgress;
            var num = 1f;
            var temperatureForCell = GenTemperature.GetTemperatureForCell(parent.PositionHeld, parent.MapHeld);
            var b = list.FirstOrDefault(x => x is Building_RottableFixer);
            if (b != null)
            {
                if (!isMeat && b.def.defName == "LotR_PantryShelf")
                {
                    num = 0.3f;
                }

                if (b.def.defName == "LotR_SaltBarrel")
                {
                    num = 0.0666667f;
                }
            }

            num *= GenTemperature.RotRateAtTemperature(temperatureForCell);
            RotProgress += Mathf.Round(num * 250f);
            if (Stage == RotStage.Rotting && PropsRot.rotDestroys)
            {
                if (parent.Position.GetSlotGroup(parent.Map) != null)
                {
                    Messages.Message("MessageRottedAwayInStorage".Translate(new object[]
                    {
                        parent.Label
                    }).CapitalizeFirst(), MessageTypeDefOf.SilentInput);
                    LessonAutoActivator.TeachOpportunity(ConceptDefOf.SpoilageAndFreezers,
                        OpportunityType.GoodToKnow);
                }

                parent.Destroy();
                return;
            }

            if (Mathf.FloorToInt(rotProgress / 60000f) == Mathf.FloorToInt(RotProgress / 60000f))
            {
                return;
            }

            if (Stage == RotStage.Rotting && PropsRot.rotDamagePerDay > 0f)
            {
                parent.TakeDamage(new DamageInfo(DamageDefOf.Rotting,
                    GenMath.RoundRandom(PropsRot.rotDamagePerDay)));
            }
            else if (Stage == RotStage.Dessicated && PropsRot.dessicatedDamagePerDay > 0f &&
                     ShouldTakeDessicateDamage())
            {
                parent.TakeDamage(new DamageInfo(DamageDefOf.Rotting,
                    GenMath.RoundRandom(PropsRot.dessicatedDamagePerDay)));
            }
        }

        private void HandleMeatThatNeedsSalting(ThingWithComps meat)
        {
            var count = meat.stackCount;
            float curHP = meat.HitPoints;
            var curDefName = meat.def.defName;
            var curPosition = meat.PositionHeld;
            var curMap = meat.MapHeld;
            parent.Destroy();
            var newThing = (ThingWithComps) ThingMaker.MakeThing(ThingDef.Named(curDefName + "Salted"));
            newThing.stackCount = count;
            newThing.HitPoints =
                Mathf.RoundToInt(curHP / meat.GetStatValue(StatDefOf.MaxHitPoints) * newThing.MaxHitPoints); // curHP;
            GenPlace.TryPlaceThing(newThing, curPosition, curMap, ThingPlaceMode.Direct);
        }

        private bool ShouldTakeDessicateDamage()
        {
            if (parent.ParentHolder == null)
            {
                return true;
            }

            if (parent.ParentHolder is Thing thing && thing.def.category == ThingCategory.Building &&
                thing.def.building.preventDeteriorationInside)
            {
                return false;
            }

            return true;
        }
    }
}