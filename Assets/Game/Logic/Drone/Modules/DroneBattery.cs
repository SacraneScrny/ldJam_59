using System;

using Game.Logic.Level;

using R3;

using Sackrany.Actor.Modules;

using UnityEngine;

namespace Game.Logic.Drone.Modules
{
    public class DroneBatteryModule : Module
    {
        public ReactiveProperty<int> Battery;
        protected override void OnStart()
        {
            Battery = new ReactiveProperty<int>(100);
            Battery.Subscribe((v) =>
            {
                if (v <= 0) GameLevelManager.MarkLose();
            });
        }
        protected override void OnReset()
        {
            Battery.Dispose();
        }

        public void Damage(float value)
        {
            Battery.Value -= Mathf.CeilToInt(value);
            Battery.Value = Mathf.Clamp(Battery.Value, 0, 100);
        }
        public void Heal(float value)
        {
            Battery.Value += Mathf.FloorToInt(value);
            Battery.Value = Mathf.Clamp(Battery.Value, 0, 100);
        }
    }

    [Serializable]
    public struct DroneBattery : ModuleTemplate<DroneBatteryModule>
    {
        
    }
}