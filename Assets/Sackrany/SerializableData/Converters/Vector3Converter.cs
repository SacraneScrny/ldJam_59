using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace Sackrany.SerializableData.Converters
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        const string TypeMeta = "$type";

        public override void WriteJson(JsonWriter writer, Vector3 v, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(TypeMeta); writer.WriteValue(TypeString);
            writer.WritePropertyName("x"); writer.WriteValue(v.x);
            writer.WritePropertyName("y"); writer.WriteValue(v.y);
            writer.WritePropertyName("z"); writer.WriteValue(v.z);
            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existing, bool hasExisting, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Vector3(
                obj["x"]?.Value<float>() ?? 0,
                obj["y"]?.Value<float>() ?? 0,
                obj["z"]?.Value<float>() ?? 0);
        }

        static string TypeString { get; } =
            $"{typeof(Vector3).FullName}, {typeof(Vector3).Assembly.GetName().Name}";
    }
}
