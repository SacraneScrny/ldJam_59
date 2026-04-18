using System;
using System.Collections.Generic;

namespace Sackrany.SerializableData.Serialization
{
    [Serializable]
    public struct SerializationData
    {
        public Dictionary<string, object> Fields;

        public SerializationData(Dictionary<string, object> fields) => Fields = fields;
    }
}
