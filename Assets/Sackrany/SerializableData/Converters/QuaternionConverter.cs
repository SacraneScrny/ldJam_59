using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace Sackrany.SerializableData.Converters
{
    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        const string TypeMeta = "$type";

        public override void WriteJson(JsonWriter writer, Quaternion v, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(TypeMeta); writer.WriteValue(TypeString);
            writer.WritePropertyName("x"); writer.WriteValue(v.x);
            writer.WritePropertyName("y"); writer.WriteValue(v.y);
            writer.WritePropertyName("z"); writer.WriteValue(v.z);
            writer.WritePropertyName("w"); writer.WriteValue(v.w);
            writer.WriteEndObject();
        }

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existing, bool hasExisting, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Quaternion(
                obj["x"]?.Value<float>() ?? 0,
                obj["y"]?.Value<float>() ?? 0,
                obj["z"]?.Value<float>() ?? 0,
                obj["w"]?.Value<float>() ?? 1);
        }

        static string TypeString { get; } =
            $"{typeof(Quaternion).FullName}, {typeof(Quaternion).Assembly.GetName().Name}";
    }
}
