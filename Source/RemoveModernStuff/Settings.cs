using UnityEngine;
using Verse;

namespace TheThirdAge;

public class Settings : ModSettings
{
    public bool LimitTechnology = true;

    public void DoWindowContents(Rect canvas)
    {
        var gap = 8f;
        var listing_Standard = new Listing_Standard
        {
            ColumnWidth = canvas.width
        };
        listing_Standard.Begin(canvas);
        listing_Standard.Gap(gap);
        listing_Standard.CheckboxLabeled("TTA_LimitTechnology".Translate(), ref LimitTechnology,
            "TTA_LimitTechnologyDescription".Translate());
        listing_Standard.Gap(gap);
        listing_Standard.Label("TTA_RestartWarning".Translate());
        listing_Standard.End();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref LimitTechnology, "LimitTechnology", true);
    }
}