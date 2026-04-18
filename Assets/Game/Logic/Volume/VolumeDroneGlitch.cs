using System;

using Game.Logic.Drone.Modules;
using Sackrany.Actor.Managers;
using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.Traits.Tags;
using Sackrany.Actor.UnitMono;

using UnityEngine;

using URPGlitch;

namespace Game.Logic.Volume
{
    public class VolumeDroneGlitchModule : Module, ILateUpdateModule
    {
        [Dependency] UnityEngine.Rendering.Volume _volume;
        [Template] VolumeDroneGlitch _template;
        
        DigitalGlitchVolume _digitalGlitch;
        Unit _drone;
        DroneSignalAffectModule _droneSignal;

        bool _hasDrone;
        
        protected override void OnStart()
        {
            _volume.profile.TryGet(out _digitalGlitch);
            UnitCmd.Execute(
                (u) => u.Tag.HasTag<Player>(), 
                (u) =>
                {
                    _drone = u;
                    _droneSignal = _drone.Get<DroneSignalAffectModule>();
                    _hasDrone = true;
                }
            );
        }
        protected override void OnReset()
        {
            _hasDrone = false;
        }
        
        float _value;
        public void OnLateUpdate(float deltaTime)
        {
            if (!_hasDrone) return;
            _value = Mathf.Lerp(_value, _droneSignal.CurrentPenalty * _template.PowerMultiply, _template.LerpSpeed * deltaTime);
            _digitalGlitch.intensity.value = _value;
        }
    }

    [Serializable]
    public struct VolumeDroneGlitch : ModuleTemplate<VolumeDroneGlitchModule>
    {
        public float LerpSpeed;
        public float PowerMultiply;
    }
}