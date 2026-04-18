using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.GameInput;

using UnityEngine;

namespace Game.Logic.Drone.Modules
{
    public class DroneControllerModule : Module, IFixedUpdateModule
    {
        [Dependency] DroneFlyModule _flyModule;
        [Dependency] DroneMissleModule _missleModule;
        
        protected override void OnStart()
        {
            _flyModule.MoveDirection.Add(() =>
            {
                return InputManager.PlayerCache.Move;
            });
        }
        public void OnFixedUpdate(float deltaTime)
        {
            if (InputManager.PlayerCache.Jump)
            {
                _missleModule.Shoot();
            }
        }
    }

    [Serializable]
    public struct DroneController : ModuleTemplate<DroneControllerModule>
    {
        
    }
}