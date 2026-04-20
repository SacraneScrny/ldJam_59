using System;

using Sackrany.Actor.DefaultFeatures.VolumeFeature;
using Sackrany.Actor.DefaultFeatures.VolumeFeature.Entities;
using Sackrany.Actor.Managers;
using Sackrany.Actor.Modules;

using UnityEngine;

namespace Game.Logic.Level
{
    public class TimeFlowVolumeModule : Module
    {
        [Dependency] VolumeModule _volume;
        protected override void OnStart()
        {
            _volume.ColorAdjustmentsVariable.Add(() => new ColorAdjustmentsVar()
            {
                saturation = (1f - UnitTimeFlowManager.TimeFlow) * -50,
                contrast = (1f - UnitTimeFlowManager.TimeFlow) * -25,
                postExposure = (1f - UnitTimeFlowManager.TimeFlow) * 2
            });
        }
    }
    
    [Serializable]
    public struct TimeFlowVolume : ModuleTemplate<TimeFlowVolumeModule> { }
}