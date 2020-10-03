using CombatExtended;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SteamWorldCE
{

    [StaticConstructorOnStartup]
    static class SteamWorldCE
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
