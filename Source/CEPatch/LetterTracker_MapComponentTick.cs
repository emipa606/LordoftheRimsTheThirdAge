using CombatExtended;
using HarmonyLib;

namespace SteamWorldCE;

[HarmonyPatch(typeof(LetterTracker), nameof(LetterTracker.MapComponentTick))]
public class LetterTracker_MapComponentTick
{
    public static void Prefix(ref bool ____sentMechWarning)
    {
        ____sentMechWarning = true;
    }
}