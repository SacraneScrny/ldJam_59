using Sackrany.Actor.UnitMono;
using Sackrany.ActorAI.BlackboardSystem.Base;

using UnityEngine;
using UnityEngine.Scripting;

namespace Sackrany.ActorAI.BlackboardSystem
{
    [Preserve] public class PlayerUnitKey     : ABbKey<PlayerUnitKey,     Unit>    { }
    [Preserve] public class PlayerPositionKey : ABbKey<PlayerPositionKey, Vector3> { }

    [Preserve] public class TargetUnitKey      : ABbKey<TargetUnitKey,      Unit>    { }
    [Preserve] public class TargetPositionKey  : ABbKey<TargetPositionKey,  Vector3> { }
    [Preserve] public class TargetDistanceKey  : ABbKey<TargetDistanceKey,  float>   { }
    [Preserve] public class TargetInSightKey   : ABbKey<TargetInSightKey,   bool>    { }
    [Preserve] public class TargetInMeleeKey   : ABbKey<TargetInMeleeKey,   bool>    { }
    [Preserve] public class TargetInRangeKey   : ABbKey<TargetInRangeKey,   bool>    { }

    [Preserve] public class HealthPercentKey   : ABbKey<HealthPercentKey,   float>   { }
    [Preserve] public class IsLowHealthKey     : ABbKey<IsLowHealthKey,     bool>    { }

    [Preserve] public class IsGroundedKey      : ABbKey<IsGroundedKey,      bool>    { }
    [Preserve] public class NearbyAllyCountKey : ABbKey<NearbyAllyCountKey, int>     { }
}