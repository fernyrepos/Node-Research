using System;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BetterResearchMenu
{
    public static class ThemingCompat
    {
        private static bool? active;
        public static bool Active => active ??= ModCompat.IsActive("ferny.themingformodpack");

        public static bool TryWarnBeforeEmergence(ResearchProjectDef def, Action onConfirmed)
        {
            if (!Active || def == null || !def.HasModExtension<EmergenceExtension>()) return false;

            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("BRM_EmergenceThreatWarning".Translate(), onConfirmed));
            SoundDefOf.Tick_Low.PlayOneShotOnCamera();
            return true;
        }
    }
}
