using System;

using Sackrany.ActorAI.BlackboardSystem.Base;
using Sackrany.Utils.CacheRegistry;

namespace Sackrany.ActorAI.BlackboardSystem.Static
{
    public static class SensorRegistry
    {
        public static int  GetId<T>() where T : ISensor => TypeRegistry<ISensor>.Id<T>.Value;
        public static int  GetId(Type type) => TypeRegistry<ISensor>.GetOrRegister(type);
        public static Type GetTypeById(int id) => TypeRegistry<ISensor>.GetTypeById(id);
    }
}