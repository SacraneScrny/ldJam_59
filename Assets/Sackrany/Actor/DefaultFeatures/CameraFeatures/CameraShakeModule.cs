using System;
using System.Collections.Generic;

using ModifiableVariable;
using ModifiableVariable.Entities;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.CustomRandom.Global;

using UnityEngine;

namespace Sackrany.Actor.DefaultFeatures.CameraFeatures
{
    public class CameraShakeModule : Module, IUpdateModule
    {
        [Dependency] UnitCameraModule _camera;
        [Template] CameraShake _template;
        
        ModifierDelegateHandler<Quaternion> _rotationDelegate;
        ModifierDelegateHandler<Vector3> _positionDelegate;
        ModifierDelegateHandler<float> _fovDelegate;
        ModifierDelegateHandler<float> _orthoDelegate;
        
        readonly List<ShakeEntity> _cameraRotationShakeQueue = new ();
        readonly List<ShakeEntity> _cameraPositionShakeQueue = new ();
        readonly List<ShakeEntity> _cameraFovShakeQueue = new ();
        readonly List<ShakeEntity> _cameraOrthoShakeQueue = new ();
        
        Quaternion _passiveRotationShake;
        Vector3 _passivePositionShake;
        float _passiveFovShake;
        float _passiveOrthoShake;

        public Modifiable<float> PassiveShakeSpeed;
        
        public Modifiable<float> PassiveRotationShake;
        public Modifiable<float> PassivePositionShake;
        public Modifiable<float> PassiveFovShake;
        public Modifiable<float> PassiveOrthoShake;
        
        protected override void OnAwake()
        {
            PassiveShakeSpeed = _template.defaultPassiveShakeSpeed;
            PassiveRotationShake = _template.defaultRotationPassiveShake;
            PassivePositionShake = _template.defaultPositionPassiveShake;
            PassiveFovShake = _template.defaultFovPassiveShake;
            PassiveOrthoShake = _template.defaultOrthoPassiveShake;
        }
        protected override void OnStart()
        {
            _rotationDelegate = _camera.CameraRotation.Add(() => _shakeRotOffset * _passiveRotationShake);
            _positionDelegate = _camera.CameraPosition.Add(() => _shakePosOffset + _passivePositionShake);
            _fovDelegate = _camera.CameraFov.Add(() => _shakeFovOffset + _passiveFovShake);
            _orthoDelegate = _camera.CameraOrthographic.Add(() => _shakeOrthoOffset + _passiveOrthoShake);
        }
        protected override void OnReset()
        {
            PassiveShakeSpeed.Clear();
            PassiveRotationShake.Clear();
            PassivePositionShake.Clear();
            PassiveFovShake.Clear();
            PassiveOrthoShake.Clear();
            _camera.CameraRotation.Remove(_rotationDelegate);
            _camera.CameraPosition.Remove(_positionDelegate);
            _camera.CameraFov.Remove(_fovDelegate);
            _camera.CameraOrthographic.Remove(_orthoDelegate);
        }
        
        Quaternion _shakeRotOffset;
        Vector3 _shakePosOffset;
        float _shakeFovOffset;
        float _shakeOrthoOffset;
        
        public void OnUpdate(float deltaTime)
        {
            UpdatePassiveShake(deltaTime);
            UpdateShakeRotLerp(deltaTime);
            UpdateShakePosLerp(deltaTime);
            UpdateShakeFovLerp(deltaTime);
            UpdateShakeOrthoLerp(deltaTime);
            
            for (int i = 0; i < _cameraRotationShakeQueue.Count; i++)
            {
                if (_cameraRotationShakeQueue[i].delay > 0)
                {
                    _cameraRotationShakeQueue[i].delay -= deltaTime;
                    continue;
                }
                _cameraRotationShakeQueue[i].duration -= deltaTime;
            }
            for (int i = _cameraRotationShakeQueue.Count - 1; i >= 0; i--)
                if (_cameraRotationShakeQueue[i].duration <= 0)
                    _cameraRotationShakeQueue.RemoveAt(i);
            
            
            for (int i = 0; i < _cameraPositionShakeQueue.Count; i++)
            {
                if (_cameraPositionShakeQueue[i].delay > 0)
                {
                    _cameraPositionShakeQueue[i].delay -= deltaTime;
                    continue;
                }
                _cameraPositionShakeQueue[i].duration -= deltaTime;
            }
            for (int i = _cameraPositionShakeQueue.Count - 1; i >= 0; i--)
                if (_cameraPositionShakeQueue[i].duration <= 0)
                    _cameraPositionShakeQueue.RemoveAt(i);
            
            for (int i = 0; i < _cameraFovShakeQueue.Count; i++)
            {
                if (_cameraFovShakeQueue[i].delay > 0)
                {
                    _cameraFovShakeQueue[i].delay -= deltaTime;
                    continue;
                }
                _cameraFovShakeQueue[i].duration -= deltaTime;
            }
            for (int i = _cameraFovShakeQueue.Count - 1; i >= 0; i--)
                if (_cameraFovShakeQueue[i].duration <= 0)
                    _cameraFovShakeQueue.RemoveAt(i);
            
            for (int i = 0; i < _cameraOrthoShakeQueue.Count; i++)
            {
                if (_cameraOrthoShakeQueue[i].delay > 0)
                {
                    _cameraOrthoShakeQueue[i].delay -= deltaTime;
                    continue;
                }
                _cameraOrthoShakeQueue[i].duration -= deltaTime;
            }

            for (int i = _cameraOrthoShakeQueue.Count - 1; i >= 0; i--)
                if (_cameraOrthoShakeQueue[i].duration <= 0)
                    _cameraOrthoShakeQueue.RemoveAt(i);
        }
        
        void UpdateShakeRotLerp(float deltaTime)
        {
            for (int i = 0; i < _cameraRotationShakeQueue.Count; i++)
            {
                if (_cameraRotationShakeQueue[i].delay > 0) continue;
                _shakeRotOffset = Quaternion.Lerp(
                    _shakeRotOffset,
                    Quaternion.Euler(_cameraRotationShakeQueue[i].GetDirOffset()),
                    15f * deltaTime);
            }
            if (_cameraRotationShakeQueue.Count == 0) _shakeRotOffset = Quaternion.Lerp(_shakeRotOffset, Quaternion.identity, 15f * deltaTime);
        }
        void UpdateShakePosLerp(float deltaTime)
        {
            for (int i = 0; i < _cameraPositionShakeQueue.Count; i++)
            {
                if (_cameraPositionShakeQueue[i].delay > 0) continue;
                _shakePosOffset = Vector3.Lerp(
                    _shakePosOffset,
                    _cameraPositionShakeQueue[i].GetDirOffset(),
                    15f * deltaTime);
            }
            if (_cameraPositionShakeQueue.Count == 0) _shakePosOffset = Vector3.Lerp(_shakePosOffset, Vector3.zero, 15f * deltaTime);
        }
        void UpdateShakeFovLerp(float deltaTime)
        {
            for (int i = 0; i < _cameraFovShakeQueue.Count; i++)
            {
                if (_cameraFovShakeQueue[i].delay > 0) continue;
                _shakeFovOffset = Mathf.Lerp(
                    _shakeFovOffset,
                    _cameraFovShakeQueue[i].GetDirOffset().x,
                    15f * deltaTime);
            }
            if (_cameraFovShakeQueue.Count == 0) _shakeFovOffset = Mathf.Lerp(_shakeFovOffset, 0, 15f * deltaTime);
        }
        void UpdateShakeOrthoLerp(float deltaTime)
        {
            for (int i = 0; i < _cameraOrthoShakeQueue.Count; i++)
            {
                if (_cameraOrthoShakeQueue[i].delay > 0) continue;

                _shakeOrthoOffset = Mathf.Lerp(
                    _shakeOrthoOffset,
                    _cameraOrthoShakeQueue[i].GetDirOffset().x,
                    15f * deltaTime);
            }

            if (_cameraOrthoShakeQueue.Count == 0)
                _shakeOrthoOffset = Mathf.Lerp(_shakeOrthoOffset, 0f, 15f * deltaTime);
        }

        float passiveShakeTimer;
        void UpdatePassiveShake(float deltaTime)
        {
            passiveShakeTimer += deltaTime * PassiveShakeSpeed;

            float t = passiveShakeTimer / 10f * Mathf.PI * 2f;

            var cx = new Vector2(Mathf.Cos(t), Mathf.Sin(t));

            float nx = Mathf.PerlinNoise(cx.x + 0.1f, cx.y + 0.7f);
            float ny = Mathf.PerlinNoise(cx.x + 2.3f, cx.y + 3.4f);
            float nz = Mathf.PerlinNoise(cx.x + 2.2f, cx.y + 2.1f);
            
            // === ROTATION ===
            Vector3 rotDir = new Vector3(nx, ny, nz);
            if (rotDir.sqrMagnitude > 0.0001f)
                rotDir.Normalize();

            Quaternion rotTarget =
                Quaternion.AngleAxis(PassiveRotationShake, rotDir);

            _passiveRotationShake =
                Quaternion.Slerp(_passiveRotationShake, rotTarget, deltaTime * 5f);

            // === POSITION ===
            Vector3 posTarget = new Vector3(nx, ny, nz) * PassivePositionShake;

            _passivePositionShake =
                Vector3.Lerp(_passivePositionShake, posTarget, deltaTime * 5f);

            // === FOV ===
            float fovNoise = Mathf.PerlinNoise(passiveShakeTimer, 42.42f);
            float fovTarget = fovNoise * PassiveFovShake;

            _passiveFovShake =
                Mathf.Lerp(_passiveFovShake, fovTarget, deltaTime * 5f);
            
            float orthoNoise = Mathf.PerlinNoise(passiveShakeTimer, 69.69f);
            float orthoTarget = orthoNoise * PassiveOrthoShake;

            _passiveOrthoShake =
                Mathf.Lerp(_passiveOrthoShake, orthoTarget, deltaTime * 5f);
        }
        
        public void RotationShake(float duration, float strength, int count = 1)
        {
            RotationShake(Vector3.zero, duration, strength, count);
        }
        public void RotationShake(Vector3 offset, float duration, float strength, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                float delay = 0;
                Vector3 dir = offset + new Vector3
                (GlobalRandom.Current.NextFloat(-90, 90), 
                    GlobalRandom.Current.NextFloat(-90, 90), 
                    GlobalRandom.Current.NextFloat(-90, 90)) * strength;
                _cameraRotationShakeQueue.Add(new ShakeEntity()
                {
                    duration = duration,
                    maxDuration = duration,
                    dir = dir,
                    delay = delay
                });
                delay += duration; 
            }
        }
        
        public void PositionShake(float duration, float strength, int count = 1)
        {
            PositionShake(Vector3.zero, duration, strength, count);
        }
        public void PositionShake(Vector3 offset, float duration, float strength, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                float delay = 0;
                Vector3 dir = (offset + new Vector3
                (GlobalRandom.Current.NextFloat(-1, 1), 
                    GlobalRandom.Current.NextFloat(-1, 1), 
                    GlobalRandom.Current.NextFloat(-1, 1))) * strength;
                _cameraPositionShakeQueue.Add(new ShakeEntity()
                {
                    duration = duration,
                    maxDuration = duration,
                    dir = dir,
                    delay = delay
                });
                delay += duration; 
            }
        }
        
        public void FovShake(float duration, float strength, int count = 1)
        {
            FovShake(0, duration, strength, count);
        }
        public void FovShake(float offset, float duration, float strength, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                float delay = 0;
                Vector3 dir = new Vector3(offset + GlobalRandom.Current.NextFloat(-1, 1), 0, 0) * strength;
                _cameraFovShakeQueue.Add(new ShakeEntity()
                {
                    duration = duration,
                    maxDuration = duration,
                    dir = dir,
                    delay = delay
                });
                delay += duration; 
            }
        }
        
        public void OrthoShake(float duration, float strength, int count = 1)
        {
            OrthoShake(0f, duration, strength, count);
        }
        public void OrthoShake(float offset, float duration, float strength, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                float delay = 0f;
                Vector3 dir = new Vector3(
                    offset + GlobalRandom.Current.NextFloat(-1f, 1f),
                    0, 0) * strength;

                _cameraOrthoShakeQueue.Add(new ShakeEntity()
                {
                    duration = duration,
                    maxDuration = duration,
                    dir = dir,
                    delay = delay
                });

                delay += duration;
            }
        }
        
        public class ShakeEntity
        {
            public Vector3 dir;
            public float duration;
            public float maxDuration;
            public float delay;
            
            public Vector3 GetDirOffset() => dir * GetStrength();
            public float GetStrength() => (duration / maxDuration);
        }
    }

    [Serializable]
    public struct CameraShake : ModuleTemplate<CameraShakeModule>
    {
        public float defaultRotationPassiveShake;
        public float defaultPositionPassiveShake;
        public float defaultOrthoPassiveShake;
        public float defaultFovPassiveShake;
        public float defaultPassiveShakeSpeed;
    }
}