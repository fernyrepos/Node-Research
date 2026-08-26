using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BetterResearchMenu
{
    public static class ResearchQueue
    {
        public static List<string> DefNames => State.researchQueue ??= new List<string>();

        public static int Count => DefNames.Count;

        public static bool Contains(ResearchProjectDef def) => def != null && DefNames.Contains(def.defName);

        public static bool CanQueue(ResearchProjectDef def, out string reason)
        {
            reason = null;
            if (def == null) return false;
            if (def.IsFinished)
            {
                reason = "BRM_QueueAlreadyFinished".Translate(def.LabelCap);
                return false;
            }
            if (def == Find.ResearchManager.currentProj)
            {
                reason = "BRM_QueueAlreadyActive".Translate(def.LabelCap);
                return false;
            }
            if (ModsConfig.AnomalyActive && def.knowledgeCategory != null)
            {
                reason = "BRM_QueueNoAnomaly".Translate();
                return false;
            }
            return true;
        }

        public static void Add(ResearchProjectDef def)
        {
            if (def != null && !Contains(def)) DefNames.Add(def.defName);
        }

        public static void Remove(ResearchProjectDef def)
        {
            if (def != null) DefNames.Remove(def.defName);
        }

        public static void Prune()
        {
            var manager = Current.Game?.researchManager;
            if (manager == null) return;

            var current = manager.currentProj;
            for (int i = DefNames.Count - 1; i >= 0; i--)
            {
                var def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(DefNames[i]);
                if (def == null || def.IsFinished || def == current) DefNames.RemoveAt(i);
            }
        }

        public static List<ResearchProjectDef> Projects()
        {
            Prune();
            var list = new List<ResearchProjectDef>(DefNames.Count);
            for (int i = 0; i < DefNames.Count; i++)
            {
                var def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(DefNames[i]);
                if (def != null) list.Add(def);
            }
            return list;
        }

        public static bool StartNext()
        {
            Prune();
            for (int i = 0; i < DefNames.Count; i++)
            {
                var def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(DefNames[i]);
                if (def == null || !def.CanStartNow) continue;

                DefNames.RemoveAt(i);
                Find.ResearchManager.SetCurrentProject(def);
                SoundDefOf.ResearchStart.PlayOneShotOnCamera();
                TutorSystem.Notify_Event("StartResearchProject");
                Messages.Message("BRM_QueueStarted".Translate(def.LabelCap), MessageTypeDefOf.PositiveEvent, false);
                return true;
            }
            return false;
        }
    }
}
