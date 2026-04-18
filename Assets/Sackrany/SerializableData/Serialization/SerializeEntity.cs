using System;

namespace Sackrany.SerializableData.Serialization
{
    internal class SerializeEntity<T> : ISerialize
    {
        readonly Func<T> _get;
        readonly Action<T> _set;

        internal SerializeEntity(Func<T> get, Action<T> set)
        {
            _get = get;
            _set = set;
        }

        public object Get() => _get();
        public void Set(object value) => _set((T)value);
    }
}
