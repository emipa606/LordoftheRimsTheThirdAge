using HarmonyLib;
using RimWorld;
using Verse;

namespace TheThirdAge
{
    public class ModStuff : Mod
    {
        public ModStuff(ModContentPack content) : base(content)
        {
                var harmony = new Harmony("rimworld.lotr.thirdage");

                harmony.Patch(AccessTools.Method(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve"), null,
                new HarmonyMethod(typeof(ModStuff), nameof(GenerateImpliedDefs_PreResolve)), null);
         
         
        }
        
        public static void GenerateImpliedDefs_PreResolve()
        {
            OnStartup.AddSaltedMeats();
        }
    }
}