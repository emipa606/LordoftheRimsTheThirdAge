using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace TheThirdAge
{
    public class SymbolResolver_AncientShrinesGroupMedieval : SymbolResolver
    {
        private const int MaxNumCaskets = 6;

        private const float SkipShrineChance = 0.25f;

        public const int MarginCells = 1;

        public static readonly IntVec2 StandardAncientShrineSize = new IntVec2(4, 3);

        public override void Resolve(ResolveParams rp)
        {
            var num = (rp.rect.Width + 1) / (StandardAncientShrineSize.x + 1);
            var num2 = (rp.rect.Height + 1) / (StandardAncientShrineSize.z + 1);
            var bottomLeft = rp.rect.BottomLeft;
            //GeneratePods(rp, num2, num, bottomLeft);
        }

        private static void GeneratePods(ResolveParams rp, int num2, int num, IntVec3 bottomLeft)
        {
            var podContentsType = rp.podContentsType;
            if (podContentsType == null)
            {
                var value = Rand.Value;
                if (value < 0.5f)
                {
                    podContentsType = null;
                }
                else if (value < 0.7f)
                {
                    podContentsType = PodContentsType.Slave;
                }
                else
                {
                    podContentsType = PodContentsType.AncientHostile;
                }
            }

            var ancientCryptosleepCasketGroupID = rp.ancientCryptosleepCasketGroupID;
            var value2 = ancientCryptosleepCasketGroupID == null
                ? Find.UniqueIDsManager.GetNextAncientCryptosleepCasketGroupID()
                : ancientCryptosleepCasketGroupID.Value;
            var num3 = 0;
            for (var i = 0; i < num2; i++)
            {
                for (var j = 0; j < num; j++)
                {
                    if (!Rand.Chance(0.25f))
                    {
                        if (num3 >= 6)
                        {
                            break;
                        }

                        var rect = new CellRect(
                            bottomLeft.x + (j * (StandardAncientShrineSize.x + 1)),
                            bottomLeft.z + (i * (StandardAncientShrineSize.z + 1)),
                            StandardAncientShrineSize.x,
                            StandardAncientShrineSize.z);
                        if (rect.FullyContainedWithin(rp.rect))
                        {
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
        }
    }
}