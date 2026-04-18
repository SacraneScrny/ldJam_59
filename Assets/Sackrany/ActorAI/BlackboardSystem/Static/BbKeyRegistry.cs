using System;

using Sackrany.ActorAI.BlackboardSystem.Base;
using Sackrany.Utils.CacheRegistry;

namespace Sackrany.ActorAI.BlackboardSystem.Static
{
    public static class BbKeyRegistry
    {
        public static int  GetId<T>() where T : IBbKey  => TypeRegistry<IBbKey>.Id<T>.Value;
        public static int  GetId(Type type)              => TypeRegistry<IBbKey>.GetOrRegister(type);
        public static Type GetTypeById(int id)           => TypeRegistry<IBbKey>.GetTypeById(id);
    }
}