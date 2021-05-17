using HarmonyLib;
using UnityEngine;
using Verse;

namespace TheThirdAge
{
    /// <summary>
    ///     Original code by Xen https://github.com/XenEmpireAdmin
    ///     Adjustments by Jecrell https://github.com/Jecrell
    /// </summary>
    [StaticConstructorOnStartup]
    public static class MainMenuTex
    {
        private static readonly bool debug = false;

        public static Texture2D BGMain;

        static MainMenuTex()
        {
            if (!ModStuff.Settings.LimitTechnology)
            {
                return;
            }

            LoadTextures();
        }

        private static void LoadTextures()
        {
            BGMain = ContentFinder<Texture2D>.Get("UI/HeroArt/TTABGPlanet");

            try
            {
                Traverse.CreateWithType("UI_BackgroundMain").Field("BGPlanet").SetValue(BGMain);
            }
            catch
            {
                if (debug)
                {
                    Log.Message("Failed to Traverse BGPlanet");
                }
            }
        }
    }
}