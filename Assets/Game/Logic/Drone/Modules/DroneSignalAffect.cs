using System;

using Game.Logic.SignalWave;

using ModifiableVariable.Stages.StageFactory;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Extensions;

using UnityEngine;

namespace Game.Logic.Drone.Modules
{
    public class DroneSignalAffectModule : Module, IFixedUpdateModule
    {
        [Template] DroneSignalAffect _template;
        [Dependency] DroneFlyModule _flyModule;

        float _squaredHitDistance;
        public float CurrentPenalty { get; private set; }
        
        float _multiplyLinearAccel = 1f;
        float _multiplyLinearDamping = 1f;
        float _multiplyAngularDamping = 1f;
        
        Vector2 _additionalLinearVelocity;
        
        protected override void OnStart()
        {
            _squaredHitDistance = _template.HitDistance * _template.HitDistance;
            _flyModule.LinearAcceleration.Add(() => _multiplyLinearAccel, General.Multiply);
            _flyModule.LinearDamping.Add(() => _multiplyLinearDamping, General.Multiply);
            _flyModule.AngularDamping.Add(() => _multiplyAngularDamping, General.Multiply);
            _flyModule.AngularAdditional.Add(() => CurrentPenalty * _additionalLinearVelocity.x, General.Multiply);
            
            _flyModule.MoveDirection.Add(() => Mathf.Pow(CurrentPenalty, 6f) * _additionalLinearVelocity);
        }
        
        public void OnFixedUpdate(float deltaTime)
        {
            CurrentPenalty = Mathf.Lerp(CurrentPenalty, 0, deltaTime * _template.PenaltyRecoverySpeed);
            
            _multiplyLinearAccel = Mathf.Lerp(_multiplyLinearAccel, 1f, deltaTime * _template.PenaltyRecoverySpeed);
            _multiplyLinearDamping = Mathf.Lerp(_multiplyLinearDamping, 1f, deltaTime * _template.PenaltyRecoverySpeed);
            _multiplyAngularDamping = Mathf.Lerp(_multiplyAngularDamping, 1f, deltaTime * _template.PenaltyRecoverySpeed);

            float t = Time.time * _template.AdditionalLinearVelocityNoiseSpeed;
            float nx = Mathf.PerlinNoise(t, t + 19.1f) * 2f - 1f;
            float ny = Mathf.PerlinNoise(t + 73.4f, t + 1000f) * 2f - 1f;
            _additionalLinearVelocity = new Vector2(nx, ny) * _template.AdditionalLinearVelocityNoiseScale;
            
            Vector3 myPos = Unit.transform.position.With(z: 0);
            for (int i = 0; i < SignalWaveManager.Instance.Elements.Count; i++)
            {
                var el = SignalWaveManager.Instance.Elements[i];
                if ((myPos - el.Position.With(z: 0)).sqrMagnitude <= _squaredHitDistance)
                {
                    _multiplyLinearAccel *= 1f / (1f + _template.LinearAccelPenalty * deltaTime);
                    _multiplyLinearDamping *= 1f / (1f + _template.LinearDampingPenalty * deltaTime);
                    _multiplyAngularDamping *= 1f / (1f + _template.AngularDampingPenalty * deltaTime);

                    CurrentPenalty += deltaTime * _template.PenaltyPower;
                    SignalWaveManager.Kill(i);
                }
            }
        }
        public override void OnDrawGizmos()
        {
            Gizmos.DrawRay(Unit.transform.position.With(z: 0), new Vector3(_additionalLinearVelocity.x, _additionalLinearVelocity.y));
        }
    }

    [Serializable]
    public struct DroneSignalAffect : ModuleTemplate<DroneSignalAffectModule>
    {
        public float HitDistance;
        public float PenaltyPower;
        public float PenaltyRecoverySpeed;
        
        public float LinearAccelPenalty;
        public float LinearDampingPenalty;
        public float AngularDampingPenalty;
        public float AdditionalLinearVelocityNoiseSpeed;
        public float AdditionalLinearVelocityNoiseScale;
    }
}