using CombatExtended;
using HarmonyLib;

namespace SteamWorldCE;

[HarmonyPatch(typeof(LetterTracker), nameof(LetterTracker.MapComponentTick))]
public class Prefix_LetterTracker_MapComponentTick
{
    [HarmonyPrefix]
    public static void Prefix(ref bool ____sentMechWarning)
    {
        ____sentMechWarning = true;
    }
}