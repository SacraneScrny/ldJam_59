using System.Collections.Generic;

using UnityEngine;

namespace Sackrany.PrefabManager
{
    /// <summary>
    /// Internal prefab loader and cache. Single flat dictionary — no generic overhead.
    /// Cache is cleared on domain reload (SubsystemRegistration).
    /// </summary>
    internal static class PrefabRegistry
    {
        static readonly Dictionary<int, GameObject> _cache = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => _cache.Clear();

        internal static GameObject Load(int hash)
        {
            if (_cache.TryGetValue(hash, out var cached))
                return cached;

            if (!PrefabManifest.Paths.TryGetValue(hash, out var path))
            {
                Debug.LogWarning($"[Prefabs] Unknown hash 0x{hash:X8}");
                return null;
            }

            var prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
                _cache[hash] = prefab;
            else
                Debug.LogWarning($"[Prefabs] Failed to load prefab at '{path}'");

            return prefab;
        }
        internal static TComponent Load<TComponent>(int hash)
        {
            var cached = Load(hash);
            return cached != null ? cached.GetComponent<TComponent>() : default;
        }

        internal static GameObject Instantiate(int hash, Transform parent = null)
        {
            var prefab = Load(hash);
            return prefab != null ? Object.Instantiate(prefab, parent) : null;
        }

        internal static GameObject Instantiate(int hash, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var prefab = Load(hash);
            if (prefab == null) return null;
            return parent != null
                ? Object.Instantiate(prefab, position, rotation, parent)
                : Object.Instantiate(prefab, position, rotation);
        }

        internal static TComponent Instantiate<TComponent>(int hash, Vector3 position, Quaternion rotation, Transform parent = null)
            where TComponent : Component
        {
            var go = Instantiate(hash, position, rotation, parent);
            return go != null ? go.GetComponent<TComponent>() : null;
        }
    }
}