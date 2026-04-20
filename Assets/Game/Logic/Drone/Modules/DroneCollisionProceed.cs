using System;

using Sackrany.Actor.Modules;
using Sackrany.GameAudio;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Game.Logic.Drone.Modules
{
    public class DroneCollisionProceedModule : Module
    {
        [Template] DroneCollisionProceed _template;
        [Dependency] DroneBatteryModule _battery;

        DroneCollider[] _colliders;
        protected override void OnStart()
        {
            _colliders = Unit.gameObject.GetComponentsInChildren<DroneCollider>();
            foreach (var c in _colliders)
                c.OnHit += OnHit;
        }
        void OnHit(Vector2 v)
        {
            _battery.Damage(v.magnitude * _template.VelocityPerDamage);
            _template.HitSounds[Random.Range(0, _template.HitSounds.Length)]
                .Prepare()
                .AtPosition(Unit.transform.position)
                .Volume(v.magnitude * _template.VelocityPerDamage * 0.4f)
                .Play();
        }
    }

    [Serializable]
    public struct DroneCollisionProceed : ModuleTemplate<DroneCollisionProceedModule>
    {
        public float VelocityPerDamage;
        public AudioClip[] HitSounds;
    }
}