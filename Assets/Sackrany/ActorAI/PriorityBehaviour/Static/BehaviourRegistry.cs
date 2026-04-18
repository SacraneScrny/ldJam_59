using System;

using Sackrany.ActorAI.PriorityBehaviour.Base;
using Sackrany.Utils.CacheRegistry;

namespace Sackrany.ActorAI.PriorityBehaviour.Static
{
    public static class BehaviourRegistry
    {
        public static int  GetId<T>() where T : IAiBehaviour => TypeRegistry<IAiBehaviour>.Id<T>.Value;
        public static int  GetId(Type type)                   => TypeRegistry<IAiBehaviour>.GetOrRegister(type);
        public static Type GetTypeById(int id)                => TypeRegistry<IAiBehaviour>.GetTypeById(id);
    }
}