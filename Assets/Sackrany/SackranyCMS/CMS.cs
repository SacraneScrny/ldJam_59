using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sackrany.CMS
{
    public static class CMS
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => Cache.ClearAll();

        public static T Get<T>(string path) where T : Object
            => Cache<T>.Get(path);
        public static void Release<T>(string path) where T : Object
            => Cache<T>.Remove(path);
        public static void ClearType<T>() where T : Object
            => Cache<T>.Clear();
        public static void ClearAll()
            => Cache.ClearAll();

        static class Cache<T> where T : Object
        {
            static readonly Dictionary<string, T> _cache = new();

            static Cache() => Cache.Register(Clear);

            internal static T Get(string path)
            {
                if (_cache.TryGetValue(path, out var cached))
                    return cached;

                var asset = Resources.Load<T>(path);
                if (asset != null)
                    _cache[path] = asset;
                else
                    Debug.LogWarning($"[CMS] Asset not found: '{path}' ({typeof(T).Name})");

                return asset;
            }

            internal static void Remove(string path) => _cache.Remove(path);
            internal static void Clear() => _cache.Clear();
        }

        static class Cache
        {
            static readonly List<System.Action> _clears = new();
            internal static void Register(System.Action clear) => _clears.Add(clear);
            internal static void ClearAll() { foreach (var c in _clears) c(); }
        }
    }
}