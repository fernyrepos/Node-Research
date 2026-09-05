using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterResearchMenu
{
    public static class DebugActions
    {
        [DebugAction("Node Research", "Advance tech level", allowedGameStates = AllowedGameStates.Playing)]
        private static void AdvanceTechLevel()
        {
            var current = Faction.OfPlayer.def.techLevel;
            if (current >= TechLevel.Archotech)
            {
                Messages.Message("Already at the highest tech level.", MessageTypeDefOf.RejectInput, false);
                return;
            }

            var target = (TechLevel)((int)current + 1);
            var proj = DefDatabase<ResearchProjectDef>.AllDefsListForReading
                .FirstOrDefault(d => !d.IsFinished && d.GetModExtension<EmergenceExtension>()?.targetLevel == target);

            if (proj == null)
            {
                Messages.Message("No unfinished emergence project targeting " + target.ToStringHuman() + ".", MessageTypeDefOf.RejectInput, false);
                return;
            }

            Find.ResearchManager.FinishProject(proj);
        }

        [DebugAction("Node Research", "Log node icon diagnostics", allowedGameStates = AllowedGameStates.Playing)]
        private static void LogNodeIconDiagnostics()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Node Research] UIScale " + Prefs.UIScale);
            AppendPatches(sb, AccessTools.Method(typeof(Widgets), nameof(Widgets.DrawTextureFitted), new[]
            {
                typeof(Rect), typeof(Texture), typeof(float), typeof(Vector2), typeof(Rect), typeof(float), typeof(Material), typeof(float)
            }), "Widgets.DrawTextureFitted");
            AppendPatches(sb, AccessTools.Method(typeof(GUIUtility), nameof(GUIUtility.RotateAroundPivot)), "GUIUtility.RotateAroundPivot");

            foreach (var proj in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
            {
                if (!(proj.UnlockedDefs.FirstOrDefault() is ThingDef t)) continue;
                if (t.uiIconAngle == 0f && t.uiIconOffset == Vector2.zero && t.uiIconScale == 1f) continue;
                sb.AppendLine(proj.defName + " -> " + t.defName
                    + " angle " + t.uiIconAngle
                    + " offset " + t.uiIconOffset
                    + " scale " + t.uiIconScale
                    + " drawSize " + (t.graphicData != null ? t.graphicData.drawSize.ToString() : "none")
                    + " size " + t.size
                    + " rot " + t.defaultPlacingRot
                    + " tex " + (t.uiIcon == null ? "null" : t.uiIcon == BaseContent.BadTex ? "bad" : t.uiIcon.width + "x" + t.uiIcon.height)
                    + " mat " + (t.uiIconMaterial != null));
            }
            Log.Message(sb.ToString());
        }

        private static void AppendPatches(StringBuilder sb, MethodBase method, string label)
        {
            if (method == null)
            {
                sb.AppendLine(label + ": not found");
                return;
            }
            var info = Harmony.GetPatchInfo(method);
            if (info == null || info.Prefixes.NullOrEmpty())
            {
                sb.AppendLine(label + ": no prefixes");
                return;
            }
            sb.AppendLine(label + " prefixes: " + string.Join(", ", info.Prefixes.Select(x => x.owner + " " + x.PatchMethod.DeclaringType?.Name).ToArray()));
        }
    }
}
