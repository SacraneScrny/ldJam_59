using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;

using UnityEngine;

namespace Game.Logic.Missle
{
    public class MissleAnimationModule : Module, ILateUpdateModule
    {
        [Template] MissleAnimation _template;
        [Dependency] MissleFlyModule _flyModule;
        
        float _time;
        float _currentRoll;
        Quaternion _originalRotation;
        MeshRenderer[] _meshRenderers;
        
        protected override void OnStart()
        {
            _meshRenderers ??= _template.ParentVisual.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in _meshRenderers)
                meshRenderer.enabled = true;
            _flyModule.OnFinished += () =>
            {
                foreach (var meshRenderer in _meshRenderers)
                    meshRenderer.enabled = false;
            };
            _currentRoll = 0;
            _time = 0;
            _originalRotation = _template.MainVisual.localRotation;
        }

        public void OnLateUpdate(float deltaTime)
        {
            _currentRoll += deltaTime * _template.RollAccel;
            _currentRoll = Mathf.Min(_currentRoll, _template.RollMaxSpeed);
            
            _time += deltaTime;
            if (_time < 1)
            {
                _template.MainVisual.localPosition = new Vector3(0, _template.StartCurvePosY.Evaluate(_time) + _currentRoll * _template.RollDistance, 0);
                _template.MainVisual.localRotation = 
                    Quaternion.Euler(0, 0, _template.StartCurveRotZ.Evaluate(_time)) 
                    * _originalRotation;
            }
            else
            {
                _template.MainVisual.localPosition = new Vector3(0, _template.StartCurvePosY.Evaluate(1) + _currentRoll * _template.RollDistance, 0);
                _template.MainVisual.localRotation =
                    Quaternion.Euler(0, 0, _template.StartCurveRotZ.Evaluate(1)) 
                    * _originalRotation;
            }
            _template.ParentVisual.localRotation *= Quaternion.Euler(_currentRoll * deltaTime, 0, 0);
        }
    }

    [Serializable]
    public struct MissleAnimation : ModuleTemplate<MissleAnimationModule>
    {
        public Transform ParentVisual;
        public Transform MainVisual;
        public AnimationCurve StartCurvePosY;
        public AnimationCurve StartCurveRotZ;

        public float RollAccel;
        public float RollMaxSpeed;
        public float RollDistance;
    }
}