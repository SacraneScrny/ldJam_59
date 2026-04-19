using System;

using Game.Logic.Camera;
using Game.Logic.Volume;

using R3;

using Sackrany.Actor.DefaultFeatures.CameraFeatures;
using Sackrany.Actor.EventBus;
using Sackrany.Actor.Managers;
using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.Static;
using Sackrany.Actor.Traits.Damage;

using UnityEngine;

using Random = UnityEngine.Random;
using Unit = Sackrany.Actor.UnitMono.Unit;

namespace Game.Logic.Missle
{
    [UpdateOrder(Order.BeforeAll)]
    public class MissleCollideModule : Module, IFixedUpdateModule
    {
        [Template] MissleCollide _template;
        [Dependency] MissleFlyModule _flyModule;
        
        CameraShakeModule _shakeModule;
        VolumeBridgeModule _volumeBridge;
        
        int _layer;
        protected override void OnAwake()
        {
            _layer = LayerMask.GetMask("Obstacle", "Enemy");    
        }
        protected override void OnStart()
        {
            CurrentDamage = new ReactiveProperty<int>();
            
            UnitCmd.Execute((u) => u.Tag.HasTag<VolumeTag>(), (u) =>
            {
                _volumeBridge = u.Get<VolumeBridgeModule>();
            });
            
            UnitCmd.Execute((u) => u.Tag.HasTag<CameraTag>(), (u) =>
            {
                _shakeModule = u.Get<CameraShakeModule>();
            });
        }
        protected override void OnReset()
        {
            CurrentDamage.Dispose();
        }
        public ReactiveProperty<int> CurrentDamage;
        public void OnFixedUpdate(float deltaTime)
        {
            if (CurrentDamage.Value == 0)
            {
                if (_flyModule.Distance >= _template.SafeDistance)
                    CurrentDamage.Value++;
            }
            else
            {
                if (_flyModule.Distance >= _template.DamageUpDistance * (CurrentDamage.Value + 1))
                    CurrentDamage.Value++;
            }
            
            var hit = Physics2D.Raycast(_template.HitFrom.position, _template.HitFrom.right, _flyModule.Speed * deltaTime, _layer);
            if (hit.collider != null)
            {
                hit.collider.GetComponentInParent<Unit>()?
                    .Maybe((u) =>
                    {
                        u.Event.Publish<Events.OnDamage, DamageInfo>(new DamageInfo(
                            CurrentDamage.Value,
                            hit.point,
                            Unit.transform.position,
                            hit.collider.gameObject,
                            Unit,
                            u,
                            null
                        ));
                    });
                
                _volumeBridge.Bloom(1, -0.15f, 0.34f * CurrentDamage.Value);
                _volumeBridge.Vignette(-.5f, 0.34f * CurrentDamage.Value);
                _volumeBridge.ColorAdjustments(0.5f * CurrentDamage.Value, -1 * CurrentDamage.Value, 0.4f * CurrentDamage.Value);
                
                _shakeModule.PositionShake(
                    -Unit.transform.right,
                    Random.Range(0.05f, 0.1f) * CurrentDamage.Value,
                    Random.Range(0.2f, 0.5f) * CurrentDamage.Value,
                    Random.Range(1, 3) * CurrentDamage.Value
                    );
                _shakeModule.RotationShake(
                    new Vector3(0, 0, (Random.value - 1) * 16f),
                    Random.Range(0.05f, 0.1f) * CurrentDamage.Value,
                    Random.Range(0.01f, .1f) * CurrentDamage.Value,
                    Random.Range(1, 3) * CurrentDamage.Value
                );
                
                _flyModule.StopWorking();
            }
        }
    }

    [Serializable]
    public struct MissleCollide : ModuleTemplate<MissleCollideModule>
    {
        public Transform HitFrom;
        public float SafeDistance;
        public float DamageUpDistance;
    }
}