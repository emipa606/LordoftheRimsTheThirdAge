﻿using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TheThirdAge;

[StaticConstructorOnStartup]
public static class OnStartup
{
    private static int movedDefs;
    private static int steelDefs;

    static OnStartup()
    {
        HandleAncientShrines();

        MoveRecipesToSmithy();

        AddCramRecipes();

        ChangeSteelToIron();

        ReplaceModernResources();
    }

    private static void AddCramRecipes()
    {
        if (TTADefOf.FueledStove is not { recipes.Count: > 0 } fueldStove)
        {
            return;
        }

        if (fueldStove.recipes.Any(x => x == TTADefOf.LotR_Make_Cram))
        {
            return;
        }

        fueldStove.recipes.Add(TTADefOf.LotR_Make_Cram);
    }

    private static void HandleAncientShrines()
    {
        if (!ModStuff.Settings.LimitTechnology)
        {
            return;
        }

        if (TTADefOf.ScatterShrines is { } scatterStep)
        {
            scatterStep.genStep = new GenStep_ScatterShrinesMedieval();
        }

        if (TTADefOf.Interior_AncientTemple is { } templeInterior)
        {
            var symbolResolverInteriorAncientTempleMedieval = new SymbolResolver_Interior_AncientTempleMedieval
            {
                minRectSize = new IntVec2(4, 3)
            };
            templeInterior.resolvers = [symbolResolverInteriorAncientTempleMedieval];
        }

        if (TTADefOf.AncientShrinesGroup is not { } shrineGroup)
        {
            return;
        }

        var symbolResolverAncientShrinesGroupMedieval = new SymbolResolver_AncientShrinesGroupMedieval
        {
            minRectSize = new IntVec2(4, 3)
        };
        shrineGroup.resolvers = [symbolResolverAncientShrinesGroupMedieval];
    }

    public static void AddSaltedMeats()
    {
        var defsToAdd = new HashSet<ThingDef>();
        foreach (var td in DefDatabase<ThingDef>.AllDefs.Where(t => t.IsMeat))
        {
            //Log.Message($"Starting with meattype {td.defName}");
            if (!td.HasComp(typeof(CompRottable)))
            {
                //Log.Message($"Skipping {td.defName} as its not rottable");
                continue;
            }

            var d = new ThingDef
            {
                resourceReadoutPriority = td.resourceReadoutPriority, //ResourceCountPriority.Middle;
                category = td.category, // ThingCategory.Item;
                thingClass = td.thingClass, // typeof(ThingWithComps);
                graphicData = new GraphicData
                {
                    graphicClass = td.graphicData.graphicClass // typeof(Graphic_Single);
                },
                useHitPoints = td.useHitPoints, // true;
                selectable = td.selectable // true;
            };
            //d.SetStatBaseValue(StatDefOf.MaxHitPoints, 115f);
            d.SetStatBaseValue(StatDefOf.MaxHitPoints, td.GetStatValueAbstract(StatDefOf.MaxHitPoints) * 1.15f);
            d.altitudeLayer = td.altitudeLayer; // AltitudeLayer.Item;
            d.stackLimit = td.stackLimit; // 75;
            d.comps.Add(new CompProperties_Forbiddable());
            var rotProps = new CompProperties_Rottable
            {
                daysToRotStart = td.GetCompProperties<CompProperties_Rottable>().daysToRotStart, // 2f;
                rotDestroys = td.GetCompProperties<CompProperties_Rottable>().rotDestroys // true;
            };
            d.comps.Add(rotProps);
            d.tickerType = td.tickerType; // TickerType.Rare;
            d.SetStatBaseValue(StatDefOf.Beauty, td.GetStatValueAbstract(StatDefOf.Beauty)); // -20f
            d.alwaysHaulable = td.alwaysHaulable; // true;
            d.rotatable = td.rotatable; // false;
            d.pathCost = td.pathCost; // 15;
            d.drawGUIOverlay = td.drawGUIOverlay; // true;
            d.socialPropernessMatters = td.socialPropernessMatters; // true;

            d.modContentPack = td.modContentPack; // +

            d.category = td.category; // ThingCategory.Item;
            d.description = td.description;
            d.useHitPoints = td.useHitPoints; // true;
            d.SetStatBaseValue(StatDefOf.MaxHitPoints,
                td.GetStatValueAbstract(StatDefOf.MaxHitPoints) * 1.15f); // 65f
            d.SetStatBaseValue(StatDefOf.DeteriorationRate,
                td.GetStatValueAbstract(StatDefOf.DeteriorationRate) * 0.5f); // 3f
            d.SetStatBaseValue(StatDefOf.Mass, td.GetStatValueAbstract(StatDefOf.Mass)); // 0.025f
            d.SetStatBaseValue(StatDefOf.Flammability, td.GetStatValueAbstract(StatDefOf.Flammability)); // 0.5f
            d.SetStatBaseValue(StatDefOf.Nutrition, td.GetStatValueAbstract(StatDefOf.Nutrition) * 1.6f);
            //d.ingestible.nutrition = td.ingestible.nutrition + 0.03f;
            d.SetStatBaseValue(StatDefOf.FoodPoisonChanceFixedHuman, 0.02f);
            //d.comps.Add(new CompProperties_FoodPoisonable());
            d.BaseMarketValue = td.BaseMarketValue;
            if (d.thingCategories == null)
            {
                d.thingCategories = [];
            }

            DirectXmlCrossRefLoader.RegisterListWantsCrossRef(d.thingCategories, "LotR_MeatRawSalted", d);
            d.ingestible = new IngestibleProperties
            {
                parent = d, foodType = td.ingestible.foodType, preferability = td.ingestible.preferability
            };
            // FoodTypeFlags.Meat;
            // FoodPreferability.RawBad;
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(d.ingestible, "tasteThought",
                ThoughtDefOf.AteRawFood.defName);
            d.ingestible.ingestEffect = td.ingestible.ingestEffect;
            d.ingestible.ingestSound = td.ingestible.ingestSound;
            d.ingestible.specialThoughtDirect = td.ingestible.specialThoughtDirect;
            d.ingestible.specialThoughtAsIngredient = td.ingestible.specialThoughtAsIngredient;

            d.graphicData.texPath = td.graphicData.texPath;
            d.graphicData.color = td.graphicData.color;
            //d.thingCategories.Add(TTADefOf.LotR_MeatRawSalted);
            d.defName = $"{td.defName}Salted";
            d.label = "TTA_SaltedLabel".Translate(td.label);
            d.ingestible.sourceDef = td.ingestible.sourceDef;
            defsToAdd.Add(d);
        }

        TTADefOf.LotR_MeatRawSalted.parent = ThingCategoryDefOf.MeatRaw;
        while (defsToAdd.Count > 0)
        {
            var thingDef = defsToAdd.FirstOrDefault();
            if (thingDef != null)
            {
                if (!DefDatabase<ThingDef>.AllDefs.Contains(thingDef))
                {
                    thingDef.PostLoad();
                    DefDatabase<ThingDef>.Add(thingDef);
                    if (!TTADefOf.LotR_MeatRawSalted.childThingDefs.Contains(thingDef))
                    {
                        TTADefOf.LotR_MeatRawSalted.childThingDefs.Add(thingDef);
                    }
                }

                defsToAdd.Remove(thingDef);
            }
            else
            {
                break;
            }
        }

        DirectXmlCrossRefLoader.ResolveAllWantedCrossReferences(FailMode.Silent);
    }

    private static void MoveRecipesToSmithy()
    {
        movedDefs = 0;
        foreach (var td in DefDatabase<ThingDef>.AllDefs.Where(t =>
                     (t?.recipeMaker?.recipeUsers?.Contains(ThingDef.Named("FueledSmithy")) ?? false) ||
                     (t?.recipeMaker?.recipeUsers?.Contains(ThingDef.Named("TableMachining")) ?? false)))
        {
            //td.recipeMaker.recipeUsers.RemoveAll(x => x.defName == "TableMachining" ||
            //                                          x.defName == "FueledSmithy");
            td.recipeMaker.recipeUsers.Add(ThingDef.Named("LotR_TableSmithy"));
            movedDefs++;
        }

        foreach (var rd in DefDatabase<RecipeDef>.AllDefs.Where(d =>
                     (d.recipeUsers?.Contains(ThingDef.Named("TableMachining")) ?? false) ||
                     (d.recipeUsers?.Contains(ThingDef.Named("FueledSmithy")) ?? false)))
        {
            //rd.recipeUsers.RemoveAll(x => x.defName == "TableMachining" ||
            //                                          x.defName == "FueledSmithy");
            rd.recipeUsers.Add(ThingDef.Named("LotR_TableSmithy"));
            movedDefs++;
        }

        Log.Message($"Moved {movedDefs} from Machining Table to Smithy.");

        if (!ModStuff.Settings.LimitTechnology ||
            ModLister.GetActiveModWithIdentifier("CETeam.CombatExtended".ToLower()) == null)
        {
            return;
        }

        var ammoRecipiesToAdd = new List<string>
        {
            "MakeAmmo_LotRE_Arrow_Galadhrim",
            "MakeAmmo_LotRE_Arrow_Mirkwood",
            "MakeAmmo_LotRE_Arrow_Rivendell",
            "MakeAmmo_LotRD_Bolt"
        };
        foreach (var recipieDefName in ammoRecipiesToAdd)
        {
            var recipieDef = DefDatabase<RecipeDef>.GetNamedSilentFail(recipieDefName);

            recipieDef?.recipeUsers.Add(ThingDef.Named("ElectricSmithy"));
        }

        var things = from thing in DefDatabase<ThingDef>.AllDefs where thing.tradeTags != null select thing;
        foreach (var thing in things)
        {
            var tags = thing.tradeTags.ToArray();
            foreach (var tag in tags)
            {
                if (tag.StartsWith("CE_AutoEnableCrafting_"))
                {
                    thing.tradeTags.Remove(tag);
                }
            }
        }

        var enumerable = new List<Def> { DefDatabase<ResearchTabDef>.GetNamed("CE_Turrets") };
        var rm = Traverse.Create(typeof(DefDatabase<ResearchTabDef>)).Method("Remove", enumerable.First());
        foreach (var def in enumerable)
        {
            rm.GetValue(def);
        }
    }

    private static void ChangeSteelToIron()
    {
        if (!ModStuff.Settings.LimitTechnology)
        {
            return;
        }

        steelDefs = 0;
        foreach (var tdd in DefDatabase<ThingDef>.AllDefs.Where(tt =>
                     tt?.costList?.Any(y => y?.thingDef == ThingDefOf.Steel) ?? false))
        {
            var tempCost = tdd.costList.FirstOrDefault(z => z.thingDef == ThingDefOf.Steel);
            if (tempCost != null)
            {
                var newTempCost = new ThingDefCountClass(ThingDef.Named("LotR_Iron"), tempCost.count);
                tdd.costList.Remove(tempCost);
                tdd.costList.Add(newTempCost);
            }

            steelDefs++;
        }


        Log.Message($"Replaced {steelDefs} defs with Iron.");
    }

    private static void ReplaceModernResources()
    {
        if (!ModStuff.Settings.LimitTechnology)
        {
            return;
        }

        if (ThingDefOf.Steel?.stuffProps?.commonality >= 0.9f)
        {
            ThingDefOf.Steel.stuffProps.commonality = 0.2f;
        }

        if (ThingDefOf.Plasteel?.stuffProps?.commonality >= 0.04f)
        {
            ThingDefOf.Plasteel.stuffProps.commonality = 0.0f;
        }

        if (ThingDefOf.Uranium?.stuffProps?.commonality >= 0.04f)
        {
            ThingDefOf.Uranium.stuffProps.commonality = 0.0f;
        }

        if (ThingDef.Named("Synthread")?.stuffProps?.commonality >= 0.014f)
        {
            ThingDef.Named("Synthread").stuffProps.commonality = 0.0f;
        }

        ThingDefOf.MineableSteel.building.mineableScatterCommonality = 0.0f;
        ThingDef.Named("MineablePlasteel").building.mineableScatterCommonality = 0.0f;
        ThingDef.Named("MineableUranium").building.mineableScatterCommonality = 0.0f;
        ThingDef.Named("MineableComponentsIndustrial").building.mineableScatterCommonality = 0.0f;
        if (!(FactionDefOf.PlayerColony?.apparelStuffFilter?.Allows(ThingDef.Named("Synthread")) ?? false))
        {
            return;
        }

        FactionDefOf.PlayerColony.apparelStuffFilter = new ThingFilter();
        FactionDefOf.PlayerColony.apparelStuffFilter.SetDisallowAll();
        FactionDefOf.PlayerColony.apparelStuffFilter.SetAllow(ThingDefOf.Cloth, true);
    }
}