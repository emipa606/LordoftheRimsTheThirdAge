using System.Linq;
using RimWorld;
using Verse;

namespace TheThirdAge
{
    public class Building_RottableFixer : Building_Storage, IStoreSettingsParent
    {
        private StorageSettings curStorageSettings;

        StorageSettings IStoreSettingsParent.GetParentStoreSettings()
        {
            return curStorageSettings;
        }

        public override void PostMake()
        {
            base.PostMake();
            curStorageSettings = new StorageSettings();
            curStorageSettings.CopyFrom(def.building.fixedStorageSettings);
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.Where(x =>
                x.HasComp(typeof(CompRottable)) || def == TTADefOf.LotR_SaltBarrel &&
                (x.thingCategories?.Contains(TTADefOf.LotR_MeatRawSalted) ??
                 false)))
            {
                if (!curStorageSettings.filter.Allows(thingDef))
                {
                    curStorageSettings.filter.SetAllow(thingDef, true);
                }
            }
        }

        public override void TickRare()
        {
            base.TickRare();
            if (!Spawned || Destroyed)
            {
                return;
            }

            MakeAllHeldThingsMedievalCompRottable();
        }

        private void MakeAllHeldThingsMedievalCompRottable()
        {
            foreach (var pos in this.OccupiedRect().Cells)
            {
                foreach (var thing in pos.GetThingList(Map))
                {
                    if (!(thing is ThingWithComps thingWithComps))
                    {
                        continue;
                    }

                    var rottable = thing.TryGetComp<CompRottable>();
                    if (rottable == null || rottable is CompMedievalRottable)
                    {
                        continue;
                    }

                    var newRot = new CompMedievalRottable();
                    thingWithComps.AllComps.Remove(rottable);
                    thingWithComps.AllComps.Add(newRot);
                    newRot.props = rottable.props;
                    newRot.parent = thingWithComps;
                    newRot.RotProgress = rottable.RotProgress;
                }
            }
        }
    }
}