using System;

using Game.Logic.Camera;
using Game.Logic.Volume;

using Sackrany.Actor.DefaultFeatures.CameraFeatures;
using Sackrany.Actor.DefaultFeatures.VolumeFeature;
using Sackrany.Actor.DefaultFeatures.VolumeFeature.Entities;
using Sackrany.Actor.Managers;
using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.UnitMono;
using Sackrany.Extensions;
using Sackrany.Utils.Pool.Extensions;

using UnityEngine;

namespace Game.Logic.Drone.Modules
{
    public class DroneMissleModule : Module, IFixedUpdateModule
    {
        [Template] DroneMissle _template;
        [Dependency] DroneSignalAffectModule _signal;
        
        CameraShakeModule _shakeModule;
        VolumeBridgeModule _volumeBridge;
        
        public Vector2 AimDirection { get; private set; }
        public float CoolDown { get; private set; }

        float _volumeEffect;
        protected override void OnStart()
        {
            UnitCmd.Execute((u) => u.Tag.HasTag<VolumeTag>(), (u) =>
            {
                _volumeBridge = u.Get<VolumeBridgeModule>();
            });
            
            UnitCmd.Execute((u) => u.Tag.HasTag<CameraTag>(), (u) =>
            {
                _shakeModule = u.Get<CameraShakeModule>();
            });
        }
        public void OnFixedUpdate(float deltaTime)
        {
            _volumeEffect = Mathf.Lerp(_volumeEffect, 0, 10f * deltaTime);
            CoolDown -= deltaTime;
            
            var _signalNoise = Mathf.PerlinNoise(Time.time * _template.SignalNoiseSpeed -23411f, Time.time * _template.SignalNoiseSpeed * 21f + 93521) * 2 - 1f;
            var _signalDir = new Vector2(Mathf.Cos(_signalNoise), Mathf.Sin(_signalNoise));
            
            AimDirection = Vector2.Lerp(AimDirection, Unit.transform.right, deltaTime * _template.LerpSpeed);
            AimDirection += _signalDir * _signal.CurrentPenalty * _template.SignalPower;
        }

        public void Shoot()
        {
            if (CoolDown > 0) return;
            
            //_shakeModule.OrthoShake(0.2f, 1f, 1);
            _volumeBridge.Bloom(2, -.5f, .8f);
            _volumeBridge.Vignette(0.25f, 2f);
            _volumeBridge.ColorAdjustments(0.2f, 1f, 0.2f);
            _shakeModule.PositionShake(-Unit.transform.right.With(z: 0), 0.2f, 0.15f, 1);
            _volumeEffect = 1f;
            
            CoolDown = _template.Cooldown;
            var m = _template.MisslePrefab.POOL();
            m.transform.position = Unit.transform.position;
            m.transform.rotation = Unit.transform.rotation;
        }
    }

    [Serializable]
    public struct DroneMissle : ModuleTemplate<DroneMissleModule>
    {
        public float LerpSpeed;
        public float SignalPower;
        public float SignalNoiseSpeed;
        public float Cooldown;
        public Unit MisslePrefab;
    }
}