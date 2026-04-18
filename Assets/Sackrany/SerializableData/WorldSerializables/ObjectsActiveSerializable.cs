using System.Linq;

using Sackrany.SerializableData.Components;

using UnityEngine;

namespace Sackrany.SerializableData.WorldSerializables
{
    public class ObjectsActiveSerializable : SerializableBehaviour
    {
        [SerializeField] GameObject[] _objects;

        protected override void OnRegister()
        {
            RegisterField<bool[]>(
                key: "objects::active",
                get: () => _objects.Select(o => o.activeSelf).ToArray(),
                set: states =>
                {
                    for (int i = 0; i < states.Length; i++)
                        _objects[i].SetActive(states[i]);
                });
        }
    }
}
