using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace TheThirdAge;

public class SymbolResolver_Interior_AncientTempleMedieval : SymbolResolver
{
    private const float MechanoidsChance = 0.5f;

    private const float HivesChance = 0.45f;

    private static readonly IntRange MechanoidCountRange = new(1, 5);

    private static readonly IntRange HivesCountRange = new(1, 2);

    private static readonly IntVec2 MinSizeForShrines = new(4, 3);

    public override void Resolve(ResolveParams rp)
    {
        var list = ThingSetMakerDefOf.MapGen_AncientTempleContents.root.Generate();
        foreach (var resolveParamsSingleThingToSpawn in list)
        {
            var resolveParams = rp;
            resolveParams.singleThingToSpawn = resolveParamsSingleThingToSpawn;
            BaseGen.symbolStack.Push("thing", resolveParams);
        }

        if (rp.rect.Width >= MinSizeForShrines.x && rp.rect.Height >= MinSizeForShrines.z)
        {
            BaseGen.symbolStack.Push("ancientShrinesGroup", rp);
        }
    }
}