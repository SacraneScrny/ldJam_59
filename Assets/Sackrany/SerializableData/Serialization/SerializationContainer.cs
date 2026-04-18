using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sackrany.SerializableData.Components;

namespace Sackrany.SerializableData.Serialization
{
    [Serializable]
    internal class SerializationContainer
    {
        [JsonProperty]
        Dictionary<string, SerializationData> _persistentData = new();

        [JsonIgnore]
        internal Dictionary<string, SerializableBehaviour> TemporaryContainer { get; } = new();

        internal void SerializeAll()
        {
            foreach (var (guid, behaviour) in TemporaryContainer)
            {
                behaviour.Serialize();
                _persistentData[guid] = new SerializationData(behaviour.SerializedFields);
            }
        }

        internal void DeserializeOne(SerializableBehaviour behaviour)
        {
            var cache = _persistentData.TryGetValue(behaviour.Guid, out var data) ? data.Fields : null;
            behaviour.Deserialize(cache);
        }

        internal void UpdateOne(SerializableBehaviour behaviour)
            => _persistentData[behaviour.Guid] = new SerializationData(behaviour.SerializedFields);
    }
}
