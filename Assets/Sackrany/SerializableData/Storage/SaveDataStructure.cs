using System.Collections.Generic;

namespace Sackrany.SerializableData.Storage
{
    public class SaveDataStructure
    {
        public Dictionary<string, object> Data = new();

        public object this[string key]
        {
            get => Data.TryGetValue(key, out var value) ? value : null;
            set => Data[key] = value;
        }

        public bool TryGetValue(string key, out object value)
            => Data.TryGetValue(key, out value);
    }
}
