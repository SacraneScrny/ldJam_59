using Sackrany.PrefabManager.Entities;

using UnityEngine;

namespace Sackrany.PrefabManager
{
    public static class PrefabRefExtensions
    {
        public static GameObject Load(this PrefabRef prefab)
            => PrefabRegistry.Load(prefab.Hash);
        public static TComponent Load<TComponent>(this PrefabRef prefab)
            => PrefabRegistry.Load<TComponent>(prefab.Hash);

        public static GameObject Instantiate(this PrefabRef prefab, Transform parent = null)
            => PrefabRegistry.Instantiate(prefab.Hash, parent);
        public static GameObject Instantiate(this PrefabRef prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            => PrefabRegistry.Instantiate(prefab.Hash, position, rotation, parent);
        public static TComponent Instantiate<TComponent>(this PrefabRef prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where TComponent : Component
            => PrefabRegistry.Instantiate<TComponent>(prefab.Hash, position, rotation, parent);
    }
}