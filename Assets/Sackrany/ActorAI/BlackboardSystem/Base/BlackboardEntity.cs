using System.Collections.Generic;

using Sackrany.ActorAI.BlackboardSystem.Static;

namespace Sackrany.ActorAI.BlackboardSystem.Base
{
    public interface IBbKey
    {
        int Id { get; }
    }
    public interface IBbKey<TValue> : IBbKey { }
    public abstract class ABbKey<TSelf, TValue> : IBbKey<TValue>
        where TSelf : ABbKey<TSelf, TValue>
    {
        public int Id => BbKeyRegistry.GetId<TSelf>();
    }
    
    public interface IReadBlackboard
    {
        public TValue Get<TKey, TValue>()
            where TKey : ABbKey<TKey, TValue>;

        public bool TryGet<TKey, TValue>(out TValue value)
            where TKey : ABbKey<TKey, TValue>;

        public bool Has<TKey>() where TKey : IBbKey;
    }
    public interface IWriteBlackboard
    {
        public void Set<TKey, TValue>(TValue value)
            where TKey : ABbKey<TKey, TValue>;
        
        public void Clear<TKey>() where TKey : IBbKey;
    }
    
    public sealed class Blackboard : IReadBlackboard, IWriteBlackboard
    {
        readonly Dictionary<int, object> _data = new();

        public void Set<TKey, TValue>(TValue value)
            where TKey : ABbKey<TKey, TValue>
            => _data[BbKeyRegistry.GetId<TKey>()] = value;

        public TValue Get<TKey, TValue>()
            where TKey : ABbKey<TKey, TValue>
        {
            _data.TryGetValue(BbKeyRegistry.GetId<TKey>(), out var v);
            return v != null ? (TValue)v : default;
        }

        public bool TryGet<TKey, TValue>(out TValue value)
            where TKey : ABbKey<TKey, TValue>
        {
            if (_data.TryGetValue(BbKeyRegistry.GetId<TKey>(), out var v))
            {
                value = (TValue)v;
                return true;
            }
            value = default;
            return false;
        }

        public bool Has<TKey>() where TKey : IBbKey
            => _data.ContainsKey(BbKeyRegistry.GetId<TKey>());

        public void Clear<TKey>() where TKey : IBbKey
            => _data.Remove(BbKeyRegistry.GetId<TKey>());

        public void ClearAll()
            => _data.Clear();
    }
}