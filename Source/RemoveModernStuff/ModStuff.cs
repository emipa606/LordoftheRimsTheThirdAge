using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace TheThirdAge;

public class ModStuff : Mod
{
    public static Settings Settings;

    public ModStuff(ModContentPack content) : base(content)
    {
        Settings = GetSettings<Settings>();
        new Harmony("rimworld.lotr.thirdage").Patch(
            AccessTools.Method(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve"), null,
            new HarmonyMethod(typeof(ModStuff), nameof(GenerateImpliedDefs_PreResolve)));
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
}