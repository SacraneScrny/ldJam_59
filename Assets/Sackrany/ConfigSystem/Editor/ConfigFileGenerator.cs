using System;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

using Sackrany.Utils.CacheRegistry;

using UnityEngine;

using static Sackrany.ConfigSystem.ConfigRegistry;

namespace Sackrany.ConfigSystem.Editor
{
    public static class ConfigFileGenerator
    {
         #if UNITY_EDITOR
        [UnityEditor.MenuItem("Sackrany/Configs/Save All")]
        static void SaveAllEditor()
        {
            var dir = "Assets/Resources/Configs";
            System.IO.Directory.CreateDirectory(dir);

            for (int i = 0; i < TypeRegistry<IConfig>.Count; i++)
            {
                var type = TypeRegistry<IConfig>.GetTypeById(i);
                var instance = GetInstanceByType(type);
                var json = JsonConvert.SerializeObject(instance, Formatting.Indented);
                System.IO.File.WriteAllText($"{dir}/{type.Name}.json", json);
            }

            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("[ConfigLoader] All configs saved to Resources/Configs");
        }
        
        [UnityEditor.MenuItem("Sackrany/Configs/Generate Defaults")]
        static void GenerateAllEditor()
        {
            var dir = "Assets/Resources/Configs";
            System.IO.Directory.CreateDirectory(dir);
            
            var configType = typeof(IConfig);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(t => t != null);
                    }
                })
                .Where(t => t.IsClass && !t.IsAbstract && configType.IsAssignableFrom(t))
                .ToList();

            int cnt = 0;
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var json = JsonConvert.SerializeObject(instance, Formatting.Indented);
                
                var path = $"{dir}/{type.Name}.json";
                if (System.IO.File.Exists(path)) continue;
                System.IO.File.WriteAllText(path, json);
                cnt++;
            }

            UnityEditor.AssetDatabase.Refresh();
            Debug.Log($"[ConfigLoader] All configs generated ({cnt}) to Resources/Configs");
        }
        #endif
    }
}