using System.Reflection;
using CombatExtended;
using HarmonyLib;
using Verse;

namespace SteamWorldCE
{
    [StaticConstructorOnStartup]
    internal static class SteamWorldCE
    {
        static SteamWorldCE()
        {
            var harmony = new Harmony("Mlie.CEPatch");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(LetterTracker))]
        [HarmonyPatch("MapComponentTick")]
        public class Prefix_LetterTracker_MapComponentTick
        {
            [HarmonyPrefix]
            public static void Prefix(ref bool ____sentMechWarning)
            {
                ____sentMechWarning = true;
            }
        }
    }
}