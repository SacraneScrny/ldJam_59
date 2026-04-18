using System.Collections.Generic;

namespace Sackrany.PrefabManager
{
    internal static class PrefabManifest
    {
        static Dictionary<int, string> _paths = new();
        internal static IReadOnlyDictionary<int, string> Paths => _paths;
        internal static void Register(Dictionary<int, string> paths) => _paths = paths;
    }
}