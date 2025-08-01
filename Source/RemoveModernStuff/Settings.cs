using UnityEngine;
using Verse;

namespace TheThirdAge;

public class Settings : ModSettings
{
    public bool LimitTechnology = true;

    public void DoWindowContents(Rect canvas)
    {
        var gap = 8f;
        var listingStandard = new Listing_Standard
        {
            ColumnWidth = canvas.width
        };
        listingStandard.Begin(canvas);
        listingStandard.Gap(gap);
        listingStandard.CheckboxLabeled("TTA_LimitTechnology".Translate(), ref LimitTechnology,
            "TTA_LimitTechnologyDescription".Translate());
        listingStandard.Gap(gap);
        listingStandard.Label("TTA_RestartWarning".Translate());
        if (ModStuff.CurrentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("TTA_CurrentModVersion".Translate(ModStuff.CurrentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref LimitTechnology, "LimitTechnology", true);
    }
}