using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Sackrany.Utils.CacheRegistry;

namespace Sackrany.ConfigSystem
{
    public static class ConfigSerializer
    {
        public static string SaveAll()
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < TypeRegistry<IConfig>.Count; i++)
            {
                var type = TypeRegistry<IConfig>.GetTypeById(i);
                var instance = ConfigRegistry.GetInstanceByType(type);
                dict[type.FullName] = instance;
            }
            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }

        public static void LoadAll(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);
            foreach (var (typeName, data) in dict)
            {
                var type = Type.GetType(typeName);
                if (type == null) continue;
                var instance = ConfigRegistry.GetInstanceByType(type);
                JsonConvert.PopulateObject(data.ToString(), instance);
            }
        }
    }
}