using UnityEngine;
using Verse;

namespace TheThirdAge
{
    // Token: 0x02000012 RID: 18
    public class Settings : ModSettings
    {
        // Token: 0x06000063 RID: 99 RVA: 0x00004630 File Offset: 0x00002830
        public void DoWindowContents(Rect canvas)
        {
            float gap = 8f;
            Listing_Standard listing_Standard = new Listing_Standard
            {
                ColumnWidth = canvas.width
            };
            listing_Standard.Begin(canvas);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("TTA_LimitTechnology".Translate(), ref LimitTechnology, "TTA_LimitTechnologyDescription".Translate());
            listing_Standard.Gap(gap);
            listing_Standard.Label("TTA_RestartWarning".Translate());
            listing_Standard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref LimitTechnology, "LimitTechnology", true, false);
        }

        public bool LimitTechnology = true;
    }
}
