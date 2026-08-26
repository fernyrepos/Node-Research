using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterResearchMenu
{
    [HarmonyPatch]
    public static class VFETribals_AdvanceTechLevel_Patch
    {
        public static bool Prepare() => ModsConfig.IsActive("OskarPotocki.VFE.Tribals");
        public static MethodBase TargetMethod() => AccessTools.Method(AccessTools.TypeByName("VFETribals.GameComponent_Tribals"), "AdvanceTechLevel");

        public static bool Prefix(object __instance, out bool __state)
        {
            if (BetterResearchMenuMod.settings.disableVFETribalsAdvancement)
            {
                var prop = AccessTools.Field(__instance.GetType(), "playerTechLevel");
                prop.SetValue(__instance, Faction.OfPlayer.def.techLevel);
                __state = false;
                return false;
            }
            __state = true;
            return true;
        }

        public static void Postfix(bool __state)
        {
            if (__state) MainTabWindow_BetterResearch.RequestFastForward();
        }
    }
}
