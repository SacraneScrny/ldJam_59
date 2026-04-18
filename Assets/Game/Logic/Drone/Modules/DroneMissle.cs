using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.UnitMono;
using Sackrany.Utils.Pool.Extensions;

using UnityEngine;

namespace Game.Logic.Drone.Modules
{
    public class DroneMissleModule : Module, IFixedUpdateModule
    {
        [Template] DroneMissle _template;
        [Dependency] DroneSignalAffectModule _signal;
        public Vector2 AimDirection { get; private set; }
        public float CoolDown { get; private set; }

        public void OnFixedUpdate(float deltaTime)
        {
            CoolDown -= deltaTime;
            
            var _signalNoise = Mathf.PerlinNoise(Time.time * _template.SignalNoiseSpeed -23411f, Time.time * _template.SignalNoiseSpeed * 21f + 93521) * 2 - 1f;
            var _signalDir = new Vector2(Mathf.Cos(_signalNoise), Mathf.Sin(_signalNoise));
            
            AimDirection = Vector2.Lerp(AimDirection, Unit.transform.right, deltaTime * _template.LerpSpeed);
            AimDirection += _signalDir * _signal.CurrentPenalty * _template.SignalPower;
        }

        public void Shoot()
        {
            if (CoolDown > 0) return;
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