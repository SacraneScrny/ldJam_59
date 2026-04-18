using System;

using UnityEngine;

namespace Sackrany.PrefabManager.Entities
{
    /// <summary>
    /// Lightweight reference to a prefab asset. Obtain via generated dot-access:
    /// <code>
    /// PrefabRef goblin = Prefabs.Enemies.Goblins.goblin01;
    /// </code>
    /// </summary>
    [Serializable]
    public struct PrefabRef : IEquatable<PrefabRef>
    {
        [SerializeField] int _hash;

        public int  Hash    => _hash;
        public bool IsValid => _hash != 0;

        public PrefabRef(int hash) => _hash = hash;

        public static readonly PrefabRef None = new(0);

        public bool Equals(PrefabRef other)     => _hash == other._hash;
        public override bool Equals(object obj) => obj is PrefabRef r && Equals(r);
        public override int  GetHashCode()      => _hash;
        public static bool operator ==(PrefabRef a, PrefabRef b) => a._hash == b._hash;
        public static bool operator !=(PrefabRef a, PrefabRef b) => a._hash != b._hash;
        public override string ToString()       => $"PrefabRef(0x{_hash:X8})";
    }
}