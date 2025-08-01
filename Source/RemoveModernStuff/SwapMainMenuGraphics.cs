using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace TheThirdAge;

[StaticConstructorOnStartup]
internal static class SwapMainMenuGraphics
{
    private static readonly bool debug = false;

    static SwapMainMenuGraphics()
    {
        if (!ModStuff.Settings.LimitTechnology)
        {
            return;
        }

        var UI_BackgroundMainPatch = new Harmony("TTA.MainMenu.UI_BackgroundMainPatch");
        var methInfBackgroundOnGUI =
            AccessTools.Method(typeof(UI_BackgroundMain), nameof(UI_BackgroundMain.BackgroundOnGUI));
        var harmonyMethodPreFBackgroundOnGUI =
            new HarmonyMethod(typeof(SwapMainMenuGraphics).GetMethod(nameof(PreFBackgroundOnGUI)));
        UI_BackgroundMainPatch.Patch(methInfBackgroundOnGUI, harmonyMethodPreFBackgroundOnGUI);
        if (debug)
        {
            Log.Message("UI_BackgroundMainPatch initialized");
        }
    }

    public static bool PreFBackgroundOnGUI()
    {
        if (!ModStuff.Settings.LimitTechnology)
        {
            return true;
        }

        // Shape the BG
        var floRatio = UI.screenWidth / 2048f;
        var floHeight = 1280f * floRatio;
        var floYPos = (UI.screenHeight - floHeight) / 2f;
        var rectBG = new Rect(0f, floYPos, UI.screenWidth, floHeight);

        // Draw the BG
        GUI.DrawTexture(rectBG, Traverse.Create(typeof(UI_BackgroundMain)).Field("BGPlanet").GetValue<Texture2D>(),
            ScaleMode.ScaleToFit);

        return false;
    }
}