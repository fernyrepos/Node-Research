using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterResearchMenu
{
    [HarmonyPatch(typeof(ResearchPrerequisitesUtility), nameof(ResearchPrerequisitesUtility.UnlockedDefsGroupedByPrerequisites))]
    public static class ResearchPrerequisitesUtility_UnlockedDefsGroupedByPrerequisites_Patch
    {
        public static void Postfix(ref List<Pair<ResearchPrerequisitesUtility.UnlockedHeader, List<Def>>> __result)
        {
            if (!MainTabWindow_BetterResearch.SuppressUnlockedWithGroups || __result == null) return;

            __result = __result
                .Where(group => group.First.unlockedBy.NullOrEmpty())
                .ToList();
        }
    }
}
