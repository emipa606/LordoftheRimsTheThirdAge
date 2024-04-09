using System.Reflection;
using HarmonyLib;
using Verse;

namespace SteamWorldCE;

[StaticConstructorOnStartup]
internal static class SteamWorldCE
{
    static SteamWorldCE()
    {
        new Harmony("Mlie.SteamWorldCE").PatchAll(Assembly.GetExecutingAssembly());
    }
}