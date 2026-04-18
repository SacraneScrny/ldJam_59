using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sackrany.Utils.CacheRegistry;
using UnityEngine;

namespace Sackrany.ConfigSystem
{
    public static class DynamicConfigLoader
    {
        static string SaveDir => Path.Combine(Application.persistentDataPath, "Configs");

        internal static void LoadAll(List<Type> dynamicTypes)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer) return;
            Directory.CreateDirectory(SaveDir);

            foreach (var type in dynamicTypes)
            {
                var path = GetPath(type);
                var instance = ConfigRegistry.GetInstanceByType(type);

                if (!File.Exists(path))
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(instance, Formatting.Indented));
                    Debug.Log($"[DynamicConfigLoader] Created default save for {type.Name}");
                    continue;
                }

                var str = File.ReadAllText(path);
                JsonConvert.PopulateObject(str, instance);
                Debug.Log($"[DynamicConfigLoader] Loaded: {type.Name}\n{str}");
            }
        }

        public static void Save<T>() where T : class, IDynamicConfig
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer) return;
            var instance = ConfigRegistry.GetInstance<T>();
            File.WriteAllText(GetPath(typeof(T)), JsonConvert.SerializeObject(instance, Formatting.Indented));
        }

        public static void SaveAll()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer) return;
            var dynamicType = typeof(IDynamicConfig);

            foreach (var (type, instance) in ConfigRegistry.GetAllInstances())
            {
                if (!dynamicType.IsAssignableFrom(type))
                    continue;

                File.WriteAllText(GetPath(type), JsonConvert.SerializeObject(instance, Formatting.Indented));
            }
        }

        public static void Reset<T>() where T : class, IDynamicConfig
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer) return;
            var savePath = GetPath(typeof(T));
            var resourceJson = Resources.Load<TextAsset>($"Configs/{typeof(T).Name}");

            if (resourceJson == null)
            {
                Debug.LogWarning($"[DynamicConfigLoader] No default found in Resources for {typeof(T).Name}, saving current state as default");
                Save<T>();
                return;
            }

            if (File.Exists(savePath))
                File.Delete(savePath);

            var instance = ConfigRegistry.GetInstance<T>();
            JsonConvert.PopulateObject(resourceJson.text, instance);
            Save<T>();
        }

        static string GetPath(Type type) => Path.Combine(SaveDir, $"{type.Name}.json");
    }
}