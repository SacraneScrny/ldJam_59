using System;

using ModifiableVariable;

using R3;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;

using UnityEngine;

namespace Game.Logic.Drone.Modules
{
    public class DroneFlyVisualModule : Module, IUpdateModule
    {
        [Dependency] DroneFlyModule _fly;
        [Template] DroneFlyVisual _template;

        Vector2 _currentTilt;

        public void OnUpdate(float deltaTime)
        {
            UpdateTilt(deltaTime);
        }

        void UpdateTilt(float deltaTime)
        {
            Vector2 worldVelocity = _fly.LinearVelocity.Value;

            var localSide = new Vector2(Unit.transform.up.x, Unit.transform.up.y);
            var localRight = new Vector2(Unit.transform.right.x, Unit.transform.right.y);

            var sideSpeed = Vector2.Dot(worldVelocity, localSide);
            var rightSpeed = Vector2.Dot(worldVelocity, localRight);

            var targetRoll = -sideSpeed * _template.RollFactor;
            var targetPitch = -rightSpeed * _template.PitchFactor;

            _currentTilt.x = Mathf.LerpAngle(_currentTilt.x, targetRoll, deltaTime * _template.TiltSpeed);
            _currentTilt.y = Mathf.LerpAngle(_currentTilt.y, targetPitch, deltaTime * _template.TiltSpeed);

            _template.Visual.localRotation = Quaternion.Euler(_currentTilt.x, _currentTilt.y, 0f);
        }

        protected override void OnReset()
        {
            _currentTilt = Vector2.zero;
            _template.Visual.localRotation = Quaternion.identity;
        }
    }

    [Serializable]
    public struct DroneFlyVisual : ModuleTemplate<DroneFlyVisualModule>
    {
        public Transform Visual;
        public float PitchFactor;
        public float RollFactor;
        public float TiltSpeed;
    }
}