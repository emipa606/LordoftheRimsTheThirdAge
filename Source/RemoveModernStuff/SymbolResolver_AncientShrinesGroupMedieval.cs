using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace TheThirdAge;

public class SymbolResolver_AncientShrinesGroupMedieval : SymbolResolver
{
    private const int MaxNumCaskets = 6;

    private const float SkipShrineChance = 0.25f;

    public const int MarginCells = 1;

    public static readonly IntVec2 StandardAncientShrineSize = new IntVec2(4, 3);

    public override void Resolve(ResolveParams rp)
    {
    }

    private static void GeneratePods(ResolveParams rp, int num2, int num, IntVec3 bottomLeft)
    {
        var podContentsType = rp.podContentsType;
        if (podContentsType == null)
        {
            var value = Rand.Value;
            switch (value)
            {
                case < 0.5f:
                    break;
                case < 0.7f:
                    podContentsType = PodContentsType.Slave;
                    break;
                default:
                    podContentsType = PodContentsType.AncientHostile;
                    break;
            }
        }

        var ancientCryptosleepCasketGroupID = rp.ancientCryptosleepCasketGroupID;
        var value2 = ancientCryptosleepCasketGroupID ??
                     Find.UniqueIDsManager.GetNextAncientCryptosleepCasketGroupID();
        var num3 = 0;
        for (var i = 0; i < num2; i++)
        {
            for (var j = 0; j < num; j++)
            {
                if (Rand.Chance(SkipShrineChance))
                {
                    continue;
                }

                if (num3 >= MaxNumCaskets)
                {
                    break;
                }

                var rect = new CellRect(
                    bottomLeft.x + (j * (StandardAncientShrineSize.x + MarginCells)),
                    bottomLeft.z + (i * (StandardAncientShrineSize.z + MarginCells)),
                    StandardAncientShrineSize.x,
                    StandardAncientShrineSize.z);
                if (!rect.FullyContainedWithin(rp.rect))
                {
                    continue;
                }

                var resolveParams = rp;
                resolveParams.rect = rect;
                resolveParams.ancientCryptosleepCasketGroupID = value2;
                resolveParams.podContentsType = podContentsType;
                BaseGen.symbolStack.Push("ancientShrine", resolveParams);
                num3++;
            }
        }
    }
}