using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterResearchMenu
{
    [HarmonyPatch(typeof(MainTabWindow_Research), "AttemptBeginResearch")]
    public static class MainTabWindow_Research_AttemptBeginResearch_Patch
    {
        private static readonly MethodInfo Target = AccessTools.Method(typeof(MainTabWindow_Research), "AttemptBeginResearch");
        private static bool confirmed;

        public static bool Prepare() => ThemingCompat.Active;

        public static bool Prefix(MainTabWindow_Research __instance, ResearchProjectDef projectToStart)
        {
            if (confirmed)
            {
                confirmed = false;
                return true;
            }

            return !ThemingCompat.TryWarnBeforeEmergence(projectToStart, delegate
            {
                confirmed = true;
                Target.Invoke(__instance, new object[] { projectToStart });
            });
        }
    }
}
