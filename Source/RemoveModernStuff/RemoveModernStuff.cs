using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace TheThirdAge;

[StaticConstructorOnStartup]
public static class RemoveModernStuff
{
    public static readonly TechLevel MaxTechlevel = TechLevel.Medieval;
    private static int removedDefs;
    private static readonly StringBuilder DebugString = new StringBuilder();

    public static readonly IEnumerable<HediffDef> hediffs;
    public static readonly IEnumerable<ThingDef> things;

    static RemoveModernStuff()
    {
        if (!ModStuff.Settings.LimitTechnology)
        {
            MaxTechlevel = TechLevel.Archotech;
            return;
        }


        DebugString.AppendLine("Lord of the Rings - The Third Age - Start Removal Log");
        DebugString.AppendLine("Tech Limiter Active: Max Level = " + MaxTechlevel);
        GiveApproppriateTechLevels();

        removedDefs = 0;

        var projects =
            DefDatabase<ResearchProjectDef>.AllDefs.Where(rpd => rpd.techLevel > MaxTechlevel);
        things = new HashSet<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where(td =>
            td.techLevel > MaxTechlevel ||
            (td.researchPrerequisites?.Any(rpd => projects.Contains(rpd)) ?? false) || new[]
            {
                "Gun_Revolver", "VanometricPowerCell", "PsychicEmanator", "InfiniteChemreactor", "Joywire",
                "Painstopper"
            }.Contains(td.defName)));

        DebugString.AppendLine("RecipeDef Removal List");


        foreach (var thing in from thing in things where thing.tradeTags != null select thing)
        {
            var tags = thing.tradeTags.ToArray();
            foreach (var tag in tags)
            {
                if (tag.StartsWith("CE_AutoEnableCrafting"))
                {
                    thing.tradeTags.Remove(tag);
                }
            }
        }

        var recipeDefsToRemove = DefDatabase<RecipeDef>.AllDefs.Where(rd =>
            rd.products.Any(tcc => things.Contains(tcc.thingDef)) ||
            rd.AllRecipeUsers.All(td => things.Contains(td)) ||
            projects.Contains(rd.researchPrerequisite)).Cast<Def>().ToList();
        recipeDefsToRemove?.RemoveAll(x =>
            x.defName == "ExtractMetalFromSlag" ||
            x.defName == "SmeltWeapon" ||
            x.defName == "DestroyWeapon" ||
            x.defName == "OfferingOfPlants_Meagre" ||
            x.defName == "OfferingOfPlants_Decent" ||
            x.defName == "OfferingOfPlants_Sizable" ||
            x.defName == "OfferingOfPlants_Worthy" ||
            x.defName == "OfferingOfPlants_Impressive" ||
            x.defName == "OfferingOfMeat_Meagre" ||
            x.defName == "OfferingOfMeat_Decent" ||
            x.defName == "OfferingOfMeat_Sizable" ||
            x.defName == "OfferingOfMeat_Worthy" ||
            x.defName == "OfferingOfMeat_Impressive" ||
            x.defName == "OfferingOfMeals_Meagre" ||
            x.defName == "OfferingOfMeals_Decent" ||
            x.defName == "OfferingOfMeals_Sizable" ||
            x.defName == "OfferingOfMeals_Worthy" ||
            x.defName == "OfferingOfMeals_Impressive" ||
            x.defName == "ROMV_ExtractBloodVial" ||
            x.defName == "ROMV_ExtractBloodPack"
        );
        RemoveStuffFromDatabase(typeof(DefDatabase<RecipeDef>), recipeDefsToRemove);

        DebugString.AppendLine("ResearchProjectDef Removal List");
        RemoveStuffFromDatabase(typeof(DefDatabase<ResearchProjectDef>), projects);


        DebugString.AppendLine("Scenario Part Removal List");
        var getThingInfo =
            typeof(ScenPart_ThingCount).GetField("thingDef", BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var def in DefDatabase<ScenarioDef>.AllDefs)
        {
            foreach (var sp in def.scenario.AllParts)
            {
                if (!(sp is ScenPart_ThingCount) || !things.Contains((ThingDef)getThingInfo?.GetValue(sp)))
                {
                    continue;
                }

                def.scenario.RemovePart(sp);
                DebugString.AppendLine("- " + sp.Label + " " + ((ThingDef)getThingInfo?.GetValue(sp)).label +
                                       " from " + def.label);
            }
        }

        foreach (var thingCategoryDef in DefDatabase<ThingCategoryDef>.AllDefs)
        {
            thingCategoryDef.childThingDefs.RemoveAll(things.Contains);
        }

        DebugString.AppendLine("Stock Generator Part Cleanup");
        foreach (var tkd in DefDatabase<TraderKindDef>.AllDefs)
        {
            for (var i = tkd.stockGenerators.Count - 1; i >= 0; i--)
            {
                var stockGenerator = tkd.stockGenerators[i];

                switch (stockGenerator)
                {
                    case StockGenerator_SingleDef sd when things.Contains(Traverse.Create(sd).Field("thingDef")
                        .GetValue<ThingDef>()):
                        var def = Traverse.Create(sd).Field("thingDef")
                            .GetValue<ThingDef>();
                        tkd.stockGenerators.Remove(stockGenerator);
                        DebugString.AppendLine("- " + def.label + " from " + tkd.label +
                                               "'s StockGenerator_SingleDef");
                        break;
                    case StockGenerator_MultiDef md:
                        var thingListTraverse = Traverse.Create(md).Field("thingDefs");
                        var thingList = thingListTraverse.GetValue<List<ThingDef>>();
                        var removeList = thingList.FindAll(things.Contains);
                        removeList?.ForEach(x =>
                            DebugString.AppendLine("- " + x.label + " from " + tkd.label +
                                                   "'s StockGenerator_MultiDef"));
                        thingList.RemoveAll(things.Contains);

                        if (thingList.NullOrEmpty())
                        {
                            tkd.stockGenerators.Remove(stockGenerator);
                        }
                        else
                        {
                            thingListTraverse.SetValue(thingList);
                        }

                        break;
                }
            }
        }


        DebugString.AppendLine("IncidentDef Removal List");

        var removedDefNames = new List<string>
        {
            "Disease_FibrousMechanites",
            "Disease_SensoryMechanites",
            "ResourcePodCrash",
            "PsychicSoothe",
            "RefugeePodCrash",
            "RansomDemand",
            "MeteoriteImpact",
            "PsychicDrone",
            "ShortCircuit",
            "ShipChunkDrop",
            "OrbitalTraderArrival",
            "StrangerInBlackJoin",
            "DefoliatorShipPartCrash",
            "PsychicEmanatorShipPartCrash",
            "MechCluster",
            "CaravanArrivalTributeCollector",
            "AnimalInsanityMass"
        };

        var incidents = from IncidentDef incident in DefDatabase<IncidentDef>.AllDefs
            where removedDefNames.Contains(incident.defName)
            select incident;


        foreach (var incident in incidents)
        {
            incident.targetTags?.Clear();
            incident.baseChance = 0f;
            incident.allowedBiomes?.Clear();
            incident.earliestDay = int.MaxValue;
        }

        RemoveStuffFromDatabase(typeof(DefDatabase<IncidentDef>), incidents);

        DebugString.AppendLine("Replaced Ancient Asphalt Road / Ancient Asphalt Highway with Stone Road");
        RoadDef[] targetRoads = { RoadDefOf.AncientAsphaltRoad, RoadDefOf.AncientAsphaltHighway };
        var originalRoad = DefDatabase<RoadDef>.GetNamed("StoneRoad");

        var fieldNames = AccessTools.GetFieldNames(typeof(RoadDef));
        fieldNames.Remove("defName");
        foreach (var fi in fieldNames.Select(name => AccessTools.Field(typeof(RoadDef), name)))
        {
            var fieldValue = fi.GetValue(originalRoad);
            foreach (var targetRoad in targetRoads)
            {
                fi.SetValue(targetRoad, fieldValue);
            }
        }

        DebugString.AppendLine("Special Hediff Removal List");
        RemoveStuffFromDatabase(typeof(DefDatabase<HediffDef>), hediffs = new[] { HediffDefOf.Gunshot });

        DebugString.AppendLine("RaidStrategyDef Removal List");
        RemoveStuffFromDatabase(typeof(DefDatabase<RaidStrategyDef>),
            DefDatabase<RaidStrategyDef>.AllDefs
                .Where(rs => typeof(ScenPart_ThingCount).IsAssignableFrom(rs.workerClass)));

        //            ItemCollectionGeneratorUtility.allGeneratableItems.RemoveAll(match: things.Contains);
        //
        //            foreach (Type type in typeof(ItemCollectionGenerator_Standard).AllSubclassesNonAbstract())
        //                type.GetMethod(name: "Reset")?.Invoke(obj: null, parameters: null);

        DebugString.AppendLine("ThingDef Removal List");
        RemoveStuffFromDatabase(typeof(DefDatabase<ThingDef>), things.ToArray());

        DebugString.AppendLine("ThingSetMaker Reset");
        ThingSetMakerUtility.Reset();

        DebugString.AppendLine("TraitDef Removal List");
        RemoveStuffFromDatabase(typeof(DefDatabase<TraitDef>),
            //                                                                   { nameof(TraitDefOf.Prosthophobe), "Prosthophile" } ?
            DefDatabase<TraitDef>.AllDefs
                .Where(td => new[] { nameof(TraitDefOf.BodyPurist), "Transhumanist" }.Contains(td.defName)));

        DebugString.AppendLine("Designators Resolved Again");
        var resolveDesignatorsAgain = typeof(DesignationCategoryDef).GetMethod("ResolveDesignators",
            BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var dcd in DefDatabase<DesignationCategoryDef>.AllDefs)
        {
            resolveDesignatorsAgain?.Invoke(dcd, null);
        }

        DebugString.AppendLine("PawnKindDef Removal List");
        RemoveStuffFromDatabase(typeof(DefDatabase<PawnKindDef>),
            DefDatabase<PawnKindDef>.AllDefs
                .Where(pkd =>
                    (!pkd.defaultFactionType?.isPlayer ?? false) &&
                    (pkd.race.techLevel > MaxTechlevel || pkd.defaultFactionType?.techLevel > MaxTechlevel)));

        DebugString.AppendLine("FactionDef Removal List");
        RemoveStuffFromDatabase(typeof(DefDatabase<FactionDef>),
            DefDatabase<FactionDef>.AllDefs.Where(fd => !fd.isPlayer && fd.techLevel > MaxTechlevel));
        if (ModLister.RoyaltyInstalled)
        {
            var incident = DefDatabase<IncidentDef>.GetNamedSilentFail("CaravanArrivalTributeCollector");
            if (incident != null)
            {
                RemoveStuffFromDatabase(typeof(DefDatabase<IncidentDef>), new List<Def> { incident });
            }
        }

        DebugString.AppendLine("BackstoryDef Removal List");
        BackstoryHandler.RemoveIncompatibleBackstories(DebugString);

        DebugString.AppendLine("MapGeneratorDef Removal List");
        DebugString.AppendLine("- GenStep_SleepingMechanoids");
        DebugString.AppendLine("- GenStep_Turrets");
        DebugString.AppendLine("- GenStep_Power");
        foreach (var mgd in DefDatabase<MapGeneratorDef>.AllDefs)
        {
            mgd.genSteps.RemoveAll(gs =>
                gs.genStep is GenStep_SleepingMechanoids || gs.genStep is GenStep_Turrets ||
                gs.genStep is GenStep_Power);
        }

        DebugString.AppendLine("RuleDef Removal List");
        DebugString.AppendLine("- SymbolResolver_AncientCryptosleepCasket");
        DebugString.AppendLine("- SymbolResolver_ChargeBatteries");
        DebugString.AppendLine("- SymbolResolver_EdgeMannedMortor");
        DebugString.AppendLine("- SymbolResolver_FirefoamPopper");
        DebugString.AppendLine("- SymbolResolver_MannedMortar");
        DebugString.AppendLine("- SymbolResolver_");
        foreach (var rd in DefDatabase<RuleDef>.AllDefs)
        {
            rd.resolvers.RemoveAll(sr =>
                sr is SymbolResolver_AncientCryptosleepCasket || sr is SymbolResolver_ChargeBatteries ||
                sr is SymbolResolver_EdgeMannedMortar || sr is SymbolResolver_FirefoamPopper ||
                sr is SymbolResolver_MannedMortar || sr is SymbolResolver_OutdoorLighting);
            if (rd.resolvers.Count == 0)
            {
                rd.resolvers.Add(new SymbolResolver_AddWortToFermentingBarrels());
            }
        }

        Log.Message("Removed " + removedDefs + " modern defs");

        PawnWeaponGenerator.Reset();
        PawnApparelGenerator.Reset();

        Debug.Log(DebugString.ToString());
        DebugString = new StringBuilder();
    }

    private static void GiveApproppriateTechLevels()
    {
        DebugString.AppendLine("ElectricSmelter's tech level changed to Industrial");
        ThingDef.Named("ElectricSmelter").techLevel = TechLevel.Industrial;

        DebugString.AppendLine("ElectricCrematorium's tech level changed to Industrial");
        ThingDef.Named("ElectricCrematorium").techLevel = TechLevel.Industrial;

        DebugString.AppendLine("FueledSmithy's tech level changed to Industrial");
        ThingDef.Named("FueledSmithy").techLevel = TechLevel.Industrial;
    }

    private static void RemoveStuffFromDatabase(Type databaseType, [NotNull] IEnumerable<Def> defs)
    {
        IEnumerable<Def> enumerable = defs as Def[] ?? defs.ToArray();
        if (!enumerable.Any())
        {
            return;
        }

        var rm = Traverse.Create(databaseType).Method("Remove", enumerable.First());
        foreach (var def in enumerable)
        {
            removedDefs++;
            DebugString.AppendLine("- " + def.label);
            rm.GetValue(def);
        }
    }
}