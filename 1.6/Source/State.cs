using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterResearchMenu
{
    public enum NodeState { Hidden, Dot, Minimized, Expanded }
    [HotSwappable]
    public static class State
    {
        public static Dictionary<string, NodeState> nodeStates = [];
        public static Dictionary<string, Vector2> nodePositions = [];
        public static List<string> expandedNodeOrder = [];
        public static HashSet<string> openedNodes = [];
        public static HashSet<string> anchoredNodes = [];
        public static Dictionary<string, float> nodeScales = [];
        public static HashSet<string> seededLayoutKeys = [];
        public static TechLevel startingScenarioTechLevel = TechLevel.Undefined;
        public static TechLevel currentSavedTechLevel = TechLevel.Undefined;
        public static bool initialized = false;
        public static bool seededDefaultAnchors = false;

        public static void Clear()
        {
            nodeStates = [];
            nodePositions = [];
            expandedNodeOrder = [];
            openedNodes = [];
            anchoredNodes = [];
            nodeScales = [];
            seededLayoutKeys = [];
            startingScenarioTechLevel = TechLevel.Undefined;
            currentSavedTechLevel = TechLevel.Undefined;
            initialized = false;
            seededDefaultAnchors = false;
        }

        public static void EnsureDefaultAnchors()
        {
            if (seededDefaultAnchors) return;
            anchoredNodes ??= [];
            anchoredNodes.Add("VFET_Fire");
            seededDefaultAnchors = true;
        }

        public static void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving && Faction.OfPlayer != null)
            {
                currentSavedTechLevel = Faction.OfPlayer.def.techLevel;
            }

            Scribe_Collections.Look(ref nodeStates, "BRM_NodeStates", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref nodePositions, "BRM_NodePositions", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref expandedNodeOrder, "BRM_ExpandedNodeOrder", LookMode.Value);
            Scribe_Collections.Look(ref openedNodes, "BRM_OpenedNodes", LookMode.Value);
            Scribe_Collections.Look(ref anchoredNodes, "BRM_AnchoredNodes", LookMode.Value);
            Scribe_Collections.Look(ref nodeScales, "BRM_NodeScales", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref seededLayoutKeys, "BRM_SeededLayoutKeys", LookMode.Value);
            Scribe_Values.Look(ref startingScenarioTechLevel, "BRM_StartingScenarioTechLevel", TechLevel.Undefined);
            Scribe_Values.Look(ref currentSavedTechLevel, "BRM_CurrentSavedTechLevel", TechLevel.Undefined);
            Scribe_Values.Look(ref initialized, "BRM_Initialized", false);
            Scribe_Values.Look(ref seededDefaultAnchors, "BRM_SeededDefaultAnchors", false);

            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                nodeStates ??= [];
                nodePositions ??= [];
                expandedNodeOrder ??= [];
                openedNodes ??= [];
                anchoredNodes ??= [];
                nodeScales ??= [];
                seededLayoutKeys ??= [];

                if (currentSavedTechLevel != TechLevel.Undefined && Faction.OfPlayer != null)
                {
                    Faction.OfPlayer.def.techLevel = currentSavedTechLevel;
                }

                if (startingScenarioTechLevel == TechLevel.Undefined && Faction.OfPlayer != null)
                    startingScenarioTechLevel = Faction.OfPlayer.def.techLevel;

                EnsureDefaultAnchors();
            }
        }
    }
}
