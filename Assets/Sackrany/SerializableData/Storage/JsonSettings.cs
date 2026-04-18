using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sackrany.SerializableData.Storage
{
    internal static class JsonSettings
    {
        static List<JsonConverter> _converters = new();

        internal static void Reset() => _converters = new List<JsonConverter>();

        internal static void RegisterConverter(JsonConverter converter)
        {
            _converters.Add(converter);
            Apply();
        }

        internal static void ApplyDefaults() => Apply();

        static void Apply()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                foreach (var converter in _converters)
                    settings.Converters.Add(converter);

                return settings;
            };
        }
    }
}
