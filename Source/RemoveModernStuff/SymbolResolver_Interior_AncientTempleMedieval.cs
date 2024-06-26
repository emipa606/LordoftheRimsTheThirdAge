﻿using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace TheThirdAge;

public class SymbolResolver_Interior_AncientTempleMedieval : SymbolResolver
{
    private const float MechanoidsChance = 0.5f;

    private const float HivesChance = 0.45f;

    private static readonly IntRange MechanoidCountRange = new IntRange(1, 5);

    private static readonly IntRange HivesCountRange = new IntRange(1, 2);

    private static readonly IntVec2 MinSizeForShrines = new IntVec2(4, 3);

    public override void Resolve(ResolveParams rp)
    {
        var list = ThingSetMakerDefOf.MapGen_AncientTempleContents.root.Generate();
        foreach (var resolveParamsSingleThingToSpawn in list)
        {
            var resolveParams = rp;
            resolveParams.singleThingToSpawn = resolveParamsSingleThingToSpawn;
            BaseGen.symbolStack.Push("thing", resolveParams);
        }

        if (ModLister.AllInstalledMods.FirstOrDefault(x =>
                x.enabled && x.Name == "Lord of the Rims - Men and Beasts") != null)
        {
            SpawnGroups(rp);
        }

        if (rp.rect.Width >= MinSizeForShrines.x && rp.rect.Height >= MinSizeForShrines.z)
        {
            BaseGen.symbolStack.Push("ancientShrinesGroup", rp);
        }
    }

    private static void SpawnGroups(ResolveParams rp)
    {
        if (!Find.Storyteller.difficulty.peacefulTemples)
        {
//                if (Rand.Chance(0.5f))
//                {
//                    ResolveParams resolveParams2 = rp;
//                    int? mechanoidsCount = rp.mechanoidsCount;
//                    resolveParams2.mechanoidsCount = new int?((mechanoidsCount.HasValue)
//                        ? SymbolResolver_Interior_AncientTempleMedieval.MechanoidCountRange.RandomInRange
//                        : mechanoidsCount.Value);
//                    BaseGen.symbolStack.Push("randomMechanoidGroup", resolveParams2);
//                }
//                else if (Rand.Chance(0.45f))
//                {
//                    ResolveParams resolveParams3 = rp;
//                    int? hivesCount = rp.hivesCount;
//                    resolveParams3.hivesCount = new int?((hivesCount == null)
//                        ? SymbolResolver_Interior_AncientTempleMedieval.HivesCountRange.RandomInRange
//                        : hivesCount.Value);
//                    BaseGen.symbolStack.Push("hives", resolveParams3);
//                }
        }
    }
}