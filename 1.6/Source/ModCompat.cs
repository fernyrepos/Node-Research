using System.Collections.Generic;
using Verse;

namespace BetterResearchMenu
{
    public static class ModCompat
    {
        private static readonly Dictionary<string, bool> cache = new Dictionary<string, bool>();

        public static bool IsActive(string packageId)
        {
            if (cache.TryGetValue(packageId, out bool active)) return active;
            active = ModsConfig.IsActive(packageId)
                || ModLister.GetActiveModWithIdentifier(packageId, ignorePostfix: true) != null;
            cache[packageId] = active;
            return active;
        }
    }
}
