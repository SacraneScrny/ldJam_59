using System;
using System.Collections.Generic;

using Sackrany.SerializableData.Serialization;

using UnityEngine;

namespace Sackrany.SerializableData.Components
{
    public abstract class SerializableBehaviour : MonoBehaviour
    {
        [SerializeField] string _guid;

        public string Guid => _guid;
        public bool IsLoaded { get; private set; }

        public event Action OnDeserialized;

        readonly Dictionary<string, ISerialize> _rules = new();

        internal Dictionary<string, object> SerializedFields { get; private set; } = new();

        void OnValidate()
        {
            if (string.IsNullOrEmpty(_guid))
                RegenerateGuid();
        }

        void Awake()
        {
            OnRegister();
            DataManager.RegisterBehaviour(this);

            if (_rules.Count > 0)
                DataManager.DeserializeOne(this);

            OnAwake();
        }

        protected virtual void OnAwake() { }
        protected abstract void OnRegister();
        protected virtual void OnDeserializedInternal() { }

        protected void RegisterField<T>(string key, Func<T> get, Action<T> set)
            => _rules.TryAdd(key, new SerializeEntity<T>(get, set));

        protected void ReadyToDeserialize()
            => DataManager.DeserializeOne(this);

        public void RegenerateGuid()
            => _guid = System.Guid.NewGuid().ToString();

        internal void Serialize()
        {
            SerializedFields = new Dictionary<string, object>();
            foreach (var (key, rule) in _rules)
                SerializedFields[key] = rule.Get();
        }

        internal void Deserialize(Dictionary<string, object> cache)
        {
            if (cache != null)
                foreach (var (key, rule) in _rules)
                    if (cache.TryGetValue(key, out var value))
                        rule.Set(value);

            IsLoaded = true;
            OnDeserializedInternal();
            OnDeserialized?.Invoke();
        }
    }
}
