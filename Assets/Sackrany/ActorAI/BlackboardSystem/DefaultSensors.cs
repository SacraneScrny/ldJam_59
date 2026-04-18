using System;

using Sackrany.Actor.DefaultFeatures.BehaviourFeature.Modules;
using Sackrany.Actor.Managers;
using Sackrany.Actor.Modules;
using Sackrany.Actor.Traits.Tags;
using Sackrany.Actor.UnitMono;
using Sackrany.ActorAI.BlackboardSystem.Base;

using UnityEngine;

namespace Sackrany.ActorAI.BlackboardSystem
{
    [Serializable]
    public struct PlayerSensorData : ISensorTemplate<PlayerSensor>
    {
        
    }
    [Serializable]
    public struct TargetSensorData : ISensorTemplate<TargetSensor>
    {
        public ITag[] EnemyTags;
        public float detectionRange;
        public float losRange;
        public float meleeRange;
        public float rangedRange;
        public LayerMask losBlockers;
    }
    [Serializable]
    public struct HealthSensorData : ISensorTemplate<HealthSensor>
    {
        public float lowHealthThreshold;
    }
    [Serializable]
    public struct EnvironmentSensorData : ISensorTemplate<EnvironmentSensor>
    {
        public float allyDetectionRange;
    }
    
    public class PlayerSensor : SensorBase
    {
        Unit _player;
        public override void OnStart()
        {
            UnitCmd.Execute((u) => u.Tag.HasTag<Player>(), (u) => _player = u);
        }
        public override void OnTick(float deltaTime)
        {
            if (_player == null)
            {
                Blackboard.Clear<PlayerUnitKey>();
                Blackboard.Clear<PlayerPositionKey>();
                return;
            }

            Blackboard.Set<PlayerUnitKey,     Unit>   (_player);
            Blackboard.Set<PlayerPositionKey, Vector3>(_player.transform.position);
        }
    }
    public class TargetSensor : SensorBase
    {
        [SensorTemplate] TargetSensorData _data;
        TeamInfo _enemyTeam;
        
        public override void OnStart()
        {
            _enemyTeam = new TeamInfo(_data.EnemyTags);
        }
        public override void OnTick(float deltaTime)
        {
            var target = FindNearestEnemy();

            if (target == null)
            {
                Blackboard.Clear<TargetUnitKey>();
                Blackboard.Clear<TargetDistanceKey>();
                Blackboard.Clear<TargetInSightKey>();
                Blackboard.Clear<TargetInMeleeKey>();
                Blackboard.Clear<TargetInRangeKey>();
                return;
            }

            var selfPos   = Unit.transform.position;
            var targetPos = target.transform.position;
            var dist      = Vector3.Distance(selfPos, targetPos);

            bool inSight = dist <= _data.losRange && !Physics.Linecast(
                selfPos, targetPos, _data.losBlockers);

            if (inSight)
                Blackboard.Set<TargetPositionKey, Vector3>(targetPos);

            Blackboard.Set<TargetUnitKey,     Unit>   (target);
            Blackboard.Set<TargetDistanceKey, float>  (dist);
            Blackboard.Set<TargetInSightKey,  bool>   (inSight);
            Blackboard.Set<TargetInMeleeKey,  bool>   (dist <= _data.meleeRange);
            Blackboard.Set<TargetInRangeKey,  bool>   (dist <= _data.rangedRange);
        }

        Unit FindNearestEnemy()
        {
            var enemies = UnitRegisterManager.GetAllUnits(_enemyTeam);
            Unit nearest  = null;
            float minDist = _data.detectionRange;

            foreach (var enemy in enemies)
            {
                if (enemy == Unit) continue;
                float d = Vector3.Distance(Unit.transform.position, enemy.transform.position);
                if (d >= minDist) continue;
                minDist = d;
                nearest = enemy;
            }
            return nearest;
        }
    }
    public class HealthSensor : SensorBase
    {
        [SensorTemplate] HealthSensorData    _data;
        [Dependency]     HealthBehaviourModule _health;
        
        public override void OnStart()
        {
            _health.OnHealthChanged += OnHealthChanged;
            WriteHealth(_health.Health);
        }

        public override void OnTick(float deltaTime) { }

        void OnHealthChanged(float current)
            => WriteHealth(current);

        void WriteHealth(float current)
        {
            float pct = _health.MaxHealth > 0 ? current / _health.MaxHealth : 0f;
            Blackboard.Set<HealthPercentKey, float>(pct);
            Blackboard.Set<IsLowHealthKey,   bool> (pct < _data.lowHealthThreshold);
        }
    }
    public class EnvironmentSensor : SensorBase
    {
        [SensorTemplate] EnvironmentSensorData  _data;
        [Dependency]     MovementBehaviourModule _movement;

        public override void OnTick(float deltaTime)
        {
            var allies = UnitRegisterManager.GetAllUnits(Unit.Team);
            int count  = 0;
            foreach (var ally in allies)
            {
                if (ally == Unit) continue;
                if (Vector3.Distance(Unit.transform.position, ally.transform.position)
                    < _data.allyDetectionRange)
                    count++;
            }

            Blackboard.Set<NearbyAllyCountKey, int> (count);
            Blackboard.Set<IsGroundedKey,      bool>(_movement.IsGrounded);
        }
    }
}