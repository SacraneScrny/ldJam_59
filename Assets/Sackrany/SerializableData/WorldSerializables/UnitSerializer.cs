using System.Collections.Generic;

using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.Static;
using Sackrany.Actor.UnitMono;
using Sackrany.SerializableData.Components;

using UnityEngine;

namespace Sackrany.SerializableData.WorldSerializables
{
    [RequireComponent(typeof(Unit))]
    public class UnitSerializer : SerializableBehaviour
    {
        [SerializeField] bool _serializePosition = true;
        [SerializeField] bool _serializeRotation = true;
        [SerializeField] bool _serializeScale;

        Unit _unit;

        protected override void OnRegister()
        {
            _unit = GetComponent<Unit>();

            _unit.Command(unit =>
            {
                RegisterModuleFields(unit);
                RegisterTransformFields();
                ReadyToDeserialize();
            });
        }

        protected override void OnDeserializedInternal()
            => _unit.MarkAsDeserialized();

        void RegisterModuleFields(Unit unit)
        {
            RegisterField<Dictionary<string, object[]>>(
                key: "unit::modules",
                get: () => SerializeModules(unit),
                set: data => DeserializeModules(unit, data));
        }

        void RegisterTransformFields()
        {
            if (_serializePosition)
                RegisterField(
                    key: "unit::position",
                    get: () => transform.position,
                    set: pos => transform.position = pos + Vector3.up * 0.01f);

            if (_serializeRotation)
                RegisterField(
                    key: "unit::rotation",
                    get: () => transform.rotation,
                    set: rot => transform.rotation = rot);

            if (_serializeScale)
                RegisterField(
                    key: "unit::scale",
                    get: () => transform.localScale,
                    set: scale => transform.localScale = scale);
        }

        static Dictionary<string, object[]> SerializeModules(Unit unit)
        {
            var result = new Dictionary<string, object[]>();
            foreach (var module in unit.GetModules())
                if (module is ISerializableModule serializable)
                    result[module.GetType().Name] = serializable.Serialize();
            return result;
        }

        static void DeserializeModules(Unit unit, Dictionary<string, object[]> data)
        {
            var modules = new Dictionary<string, ISerializableModule>();
            foreach (var module in unit.GetModules())
                if (module is ISerializableModule serializable)
                    modules[module.GetType().Name] = serializable;

            foreach (var (typeName, moduleData) in data)
                if (modules.TryGetValue(typeName, out var module))
                    module.Deserialize(moduleData);
        }
    }
}
