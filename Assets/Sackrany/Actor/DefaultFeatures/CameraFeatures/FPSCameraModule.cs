using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.UnitMono;
using Sackrany.GameInput;

using UnityEngine;

namespace Sackrany.Actor.DefaultFeatures.CameraFeatures
{
    public class FPSCameraModule : Module, ILateUpdateModule
    {
        [Template] FPSCamera _template;
        [Dependency] UnitCameraModule _unitCameraModule;

        float _x;
        
        protected override void OnStart()
        {
            _unitCameraModule.CameraPosition.Add(GetPosition);
            _unitCameraModule.CameraRotation.Add(GetRotation);
            InputManager.CurrentPointer.SwitchCursorVisibility(false);
        }
        
        Vector3 GetPosition()
        {
            return _template.PlayerUnit.transform.position
                + Vector3.up * _template.VerticalOffset;
        }
        Quaternion GetRotation()
        {
            return Quaternion.Euler(_x, _template.PlayerUnit.transform.rotation.eulerAngles.y, 0);
        }
        
        public void OnLateUpdate(float deltaTime)
        {
            _x -= InputManager.CurrentPointer.Delta.y * 20 * Time.deltaTime;
            _x = Mathf.Clamp(_x, -89, 89);
            var y = InputManager.CurrentPointer.Delta.x * 20 * Time.deltaTime;
            _template.PlayerUnit.transform.rotation *= Quaternion.Euler(0, y, 0);
        }
    }

    [Serializable]
    public struct FPSCamera : ModuleTemplate<FPSCameraModule>
    {
        public Unit PlayerUnit;
        public float VerticalOffset;
    }
}