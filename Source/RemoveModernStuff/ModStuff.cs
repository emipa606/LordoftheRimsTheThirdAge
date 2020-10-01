using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace TheThirdAge
{
    public class ModStuff : Mod
    {
        public ModStuff(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
            var harmony = new Harmony("rimworld.lotr.thirdage");
            harmony.Patch(AccessTools.Method(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve"), null,
            new HarmonyMethod(typeof(ModStuff), nameof(GenerateImpliedDefs_PreResolve)), null);
        }

        public static void GenerateImpliedDefs_PreResolve()
        {
            OnStartup.AddSaltedMeats();
        }

        public override string SettingsCategory()
        {
            return "Lord of the Rims - The Third Age";
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            Settings.DoWindowContents(canvas);
        }

        public static Settings Settings;

    }
}