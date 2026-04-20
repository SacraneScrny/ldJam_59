using System;

using Game.Logic.Level;

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
            _flyModule.MoveDirection.Add(() => GameLevelManager.IsWaitingForRestart ? Vector2.zero : InputManager.PlayerCache.Move);
        }
        public void OnFixedUpdate(float deltaTime)
        {
            if (GameLevelManager.IsWaitingForRestart) return;
            
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