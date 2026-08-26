using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterResearchMenu
{
    [HarmonyPatch(typeof(ResearchManager), nameof(ResearchManager.SetCurrentProject))]
    public static class ResearchManager_SetCurrentProject_Patch
    {
        private const string ClassicalPackageId = "oskarpotocki.vfe.classical";

        private static readonly bool tribalsActive = ModCompat.IsActive("OskarPotocki.VFE.Tribals");
        private static readonly bool classicalActive = ModCompat.IsActive(ClassicalPackageId);

        public static bool Prepare() => tribalsActive || classicalActive;

        public static void Postfix(ResearchProjectDef proj)
        {
            if (proj == null || Current.ProgramState != ProgramState.Playing) return;

            if (tribalsActive && !State.seenAnimalResearchTip && proj.techLevel == TechLevel.Animal)
            {
                State.seenAnimalResearchTip = true;
                Find.WindowStack.Add(new Dialog_MessageBox("BRM_AnimalResearchTip".Translate()));
                return;
            }

            if (classicalActive && !State.seenRepublicResearchTip && IsRepublicProject(proj))
            {
                State.seenRepublicResearchTip = true;
                Find.WindowStack.Add(new Dialog_MessageBox("BRM_RepublicResearchTip".Translate()));
            }
        }

        private static bool IsRepublicProject(ResearchProjectDef proj)
        {
            var pack = proj.modContentPack;
            return pack != null && pack.PackageId.StartsWith(ClassicalPackageId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
