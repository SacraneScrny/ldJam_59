using System;

namespace Sackrany.ConfigSystem
{
    public interface IConfig { }
    public interface IDynamicConfig : IConfig { }
    
    public readonly struct ConfigGet<T> where T : class, IConfig
    {
        public static T Value => ConfigRegistry.GetInstance<T>();
    }    
    public readonly struct ConfigSet<T> where T : class, IDynamicConfig
    {
        public static void Do(Action<T> setter, bool autoSave = false)
        {
            setter(ConfigGet<T>.Value);
            if (autoSave)
                DynamicConfigLoader.Save<T>();
        }        
        public static void Do(T value, bool autoSave = false)
        {
            ConfigRegistry.SetInstance(value);
            if (autoSave)
                DynamicConfigLoader.Save<T>();
        }        
        public static void DoAndSave(Action<T> setter)
        {
            setter(ConfigGet<T>.Value);
            DynamicConfigLoader.Save<T>();
        }
        public static void DoAndSave(T value)
        {
            ConfigRegistry.SetInstance(value);
            DynamicConfigLoader.Save<T>();
        }
    }
    
    public class DefaultConfig : IConfig { }
}