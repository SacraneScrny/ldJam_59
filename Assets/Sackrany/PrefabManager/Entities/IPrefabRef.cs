using UnityEngine;

namespace Sackrany.PrefabManager.Entities
{
    public interface IPrefabRef
    {
        PrefabRef GetRef();

        GameObject Load()
            => PrefabRegistry.Load(GetRef().Hash);
        TComponent Load<TComponent>()
            => PrefabRegistry.Load<TComponent>(GetRef().Hash);

        GameObject Instantiate(Transform parent = null)
            => PrefabRegistry.Instantiate(GetRef().Hash, parent);
        GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent = null)
            => PrefabRegistry.Instantiate(GetRef().Hash, position, rotation, parent);
        TComponent Instantiate<TComponent>(Vector3 position, Quaternion rotation, Transform parent = null)
            where TComponent : Component
            => PrefabRegistry.Instantiate<TComponent>(GetRef().Hash, position, rotation, parent);
    }
}