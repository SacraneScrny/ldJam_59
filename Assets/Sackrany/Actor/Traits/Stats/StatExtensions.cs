using ModifiableVariable;

using Sackrany.Actor.Static;
using Sackrany.Actor.UnitMono;

namespace Sackrany.Actor.Traits.Stats
{
    public static class StatExtensions
    {
        public static Modifiable<float> GetStat<T>(this Unit unit) where T : IStat
            => unit.Maybe<StatHandlerModule, Modifiable<float>>(h => h.GetStat<T>());

        public static bool TryGetStat<T>(this Unit unit, out Modifiable<float> value) where T : IStat
        {
            if (unit != null && unit.IsActive && unit.TryGet(out StatHandlerModule h))
                return h.TryGetStat<T>(out value);
            value = null;
            return false;
        }

        public static float GetStatValue<T>(this Unit unit, float fallback = 0f) where T : IStat
            => unit != null && unit.IsActive && unit.TryGet(out StatHandlerModule h)
                ? h.GetValue<T>(fallback)
                : fallback;

        public static bool HasStat<T>(this Unit unit) where T : IStat
            => unit != null && unit.IsActive && unit.TryGet(out StatHandlerModule h) && h.HasStat<T>();
    }
}