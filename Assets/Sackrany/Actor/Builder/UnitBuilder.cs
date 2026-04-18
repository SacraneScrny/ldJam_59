using UnityEngine;

namespace Sackrany.Actor.Builder
{
    public static class UnitBuilder
    {
        public static UnitBuildChain AsUnit(this GameObject go)
            => new UnitBuildChain(go);
    }
}