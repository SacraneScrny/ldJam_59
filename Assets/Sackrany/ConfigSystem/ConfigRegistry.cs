using System;
using System.Collections.Generic;
using System.Linq;

using Sackrany.Utils.CacheRegistry;

using UnityEngine;

namespace Sackrany.ConfigSystem
{
    public static class ConfigRegistry
    {
        static readonly Dictionary<Type, IConfig> _instances = new();
        static readonly object _lock = new();
        
        public static int GetId<T>() where T : IConfig => TypeRegistry<IConfig>.Id<T>.Value;
        public static T GetInstance<T>() where T : class, IConfig => (T)GetOrCreateInstance(typeof(T));
        public static void SetInstance<T>(T instance) where T : class, IConfig
        {
            lock (_lock)
            {
                GetOrCreateInstance(typeof(T));
                _instances[typeof(T)] = instance;
            }
        }
        
        static class ConfigId<T> where T : IConfig
        {
            public static readonly int Value = TypeRegistry<IConfig>.Id<T>.Value;
        }
        
        public static IEnumerable<(Type type, IConfig instance)> GetAllInstances()
        {
            lock (_lock)
                return _instances.Select(kv => (kv.Key, kv.Value)).ToList();
        }
        static IConfig GetOrCreateInstance(Type type)
        {
            lock (_lock)
            {
                if (_instances.TryGetValue(type, out var inst))
                    return inst;

                inst = Activator.CreateInstance(type) as IConfig;
                _instances[type] = inst;
                return _instances[type];
            }
        }
        public static IConfig GetInstanceByType(Type type)
        {
            lock (_lock)
            {
                if (_instances.TryGetValue(type, out var inst))
                    return inst;
                inst = Activator.CreateInstance(type) as IConfig;
                _instances[type] = inst;
                return inst;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            lock (_lock)
                _instances.Clear();
        }
    }
}