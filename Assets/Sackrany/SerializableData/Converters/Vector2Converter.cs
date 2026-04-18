using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace Sackrany.SerializableData.Converters
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        const string TypeMeta = "$type";

        public override void WriteJson(JsonWriter writer, Vector2 v, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(TypeMeta); writer.WriteValue(TypeString);
            writer.WritePropertyName("x"); writer.WriteValue(v.x);
            writer.WritePropertyName("y"); writer.WriteValue(v.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existing, bool hasExisting, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Vector2(
                obj["x"]?.Value<float>() ?? 0,
                obj["y"]?.Value<float>() ?? 0);
        }

        static string TypeString { get; } =
            $"{typeof(Vector2).FullName}, {typeof(Vector2).Assembly.GetName().Name}";
    }
}
