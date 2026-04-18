using System;
using System.Collections.Generic;

namespace Sackrany.SerializableData.Storage
{
    public class SaveSlotMeta
    {
        public int Slot { get; set; }
        public DateTime SavedAt { get; set; }
        public TimeSpan PlayTime { get; set; }
        public byte[] Thumbnail { get; set; }
        public Dictionary<string, object> Custom { get; set; } = new();

        public bool Exists => SavedAt != default;

        public void SetCustom(string key, object value) => Custom[key] = value;

        public T GetCustom<T>(string key)
            => Custom.TryGetValue(key, out var value) && value is T typed ? typed : default;
    }
}
