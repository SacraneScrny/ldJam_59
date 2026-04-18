using System;

using Game.Logic.Level.Components;

using Sackrany.Actor.DefaultFeatures.CameraFeatures;
using Sackrany.Actor.Managers;
using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.Traits.Tags;
using Sackrany.Actor.UnitMono;

using UnityEngine;

namespace Game.Logic.Camera
{
    public class CameraDroneFollowModule : Module, IUpdateModule
    {
        [Template] CameraDroneFollow _template;
        [Dependency] UnitCameraModule _camera;

        Vector3 _currentCameraOffset;
        float _currentOrthoOffset;
        Unit _player;
        
        protected override void OnStart()
        {
            _camera.CameraOrthographic.Add(() => -_currentOrthoOffset);
            
            UnitCmd.Execute(
                (u) => u.Tag.HasTag<Player>(), 
                (u) =>
                {
                    _player = u;
                }
            );
            _camera.CameraPosition.Add(() => _currentCameraOffset);
        }
        public void OnUpdate(float deltaTime)
        {
            var rect = LevelGenerator.Instance.LevelRect;
            _currentOrthoOffset = rect.size.magnitude * _template.ScaleByRect;
            
            var orig = _camera.CameraPosition.BaseValue;
            var player = (_player ?? Unit).transform.position;
            _currentCameraOffset = Vector3.Lerp(_currentCameraOffset, Vector3.Lerp(orig, player, _template.ToPlayerLerpCoef), _template.Speed * deltaTime);
        }
    }

    [Serializable]
    public struct CameraDroneFollow : ModuleTemplate<CameraDroneFollowModule>
    {
        public float ScaleByRect;
        public float Speed;
        public float ToPlayerLerpCoef;
    }
}