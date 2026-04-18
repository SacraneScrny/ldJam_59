using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Sackrany.Utils.CacheRegistry;

using UnityEngine;

namespace Sackrany.ConfigSystem
{
    public static class ConfigLoader
    {
        static Dictionary<string, Type> _nameToType;

        static List<Type> BuildTypeMap()
        {
            _nameToType = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var configType = typeof(IConfig);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && configType.IsAssignableFrom(type))
                        _nameToType[type.Name] = type;
                }
            }

            return _nameToType.Values.ToList();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void LoadAll()
        {
            var allTypes = BuildTypeMap();

            var assets = Resources.LoadAll<TextAsset>("Configs");

            if (assets.Length == 0)
            {
                Debug.LogWarning("[ConfigLoader] No config files found in Resources/Configs");
            }
            else
            {
                foreach (var asset in assets)
                {
                    if (!_nameToType.TryGetValue(asset.name, out var type))
                    {
                        Debug.LogWarning($"[ConfigLoader] No IConfig type found for: {asset.name}");
                        continue;
                    }

                    var instance = ConfigRegistry.GetInstanceByType(type);
                    JsonConvert.PopulateObject(asset.text, instance);
                    Debug.Log($"[ConfigLoader] Loaded: {type.Name}");
                }
            }

            var dynamicTypes = allTypes
                .Where(t => typeof(IDynamicConfig).IsAssignableFrom(t))
                .ToList();

            DynamicConfigLoader.LoadAll(dynamicTypes);
        }
    }
}