using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace Sackrany.SerializableData.Converters
{
    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        const string TypeMeta = "$type";

        public override void WriteJson(JsonWriter writer, Vector2Int v, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(TypeMeta); writer.WriteValue(TypeString);
            writer.WritePropertyName("x"); writer.WriteValue(v.x);
            writer.WritePropertyName("y"); writer.WriteValue(v.y);
            writer.WriteEndObject();
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existing, bool hasExisting, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Vector2Int(
                obj["x"]?.Value<int>() ?? 0,
                obj["y"]?.Value<int>() ?? 0);
        }

        static string TypeString { get; } =
            $"{typeof(Vector2Int).FullName}, {typeof(Vector2Int).Assembly.GetName().Name}";
    }
}
