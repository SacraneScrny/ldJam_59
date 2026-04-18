using System;

using Game.Logic.Level.Components;

using Sackrany.Actor.DefaultFeatures.CameraFeatures;
using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;

using UnityEngine;

namespace Game.Logic.Camera
{
    public class CameraToLevelCorrectionsModule : Module, IUpdateModule
    {
        [Dependency] UnitCameraModule _camera;
        [Template] CameraToLevelCorrections _template;

        Vector3 _pos;
        float _orthoSize;
        
        protected override void OnStart()
        {
            _camera.CameraPosition.Add(() => _pos);
            _camera.CameraOrthographic.Add(() => _orthoSize);
        }
        
        public void OnUpdate(float deltaTime)
        {
            _pos = Vector3.Lerp(_pos, LevelGenerator.Instance.LevelRect.center - Vector2.one * LevelGenerator.Instance.CellSize, 5 * deltaTime);
            
            var rect = LevelGenerator.Instance.LevelRect;
            float sizeByWidth  = rect.width  / (2f * _camera.Camera.aspect);
            float sizeByHeight = rect.height / 2f;
            
            _orthoSize = Mathf.Lerp(_orthoSize, Mathf.Max(sizeByWidth, sizeByHeight) - _camera.CameraOrthographic.BaseValue + _template.Offset / 2f, 5 * deltaTime);
        }
    }

    [Serializable]
    public struct CameraToLevelCorrections : ModuleTemplate<CameraToLevelCorrectionsModule>
    {
        public float Offset;
    }
}