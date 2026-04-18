using System;

using ModifiableVariable;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.Static;
using Sackrany.Actor.Traits.Conditions;
using Sackrany.Actor.Traits.Stats;
using Sackrany.Extensions;
using Sackrany.Variables.Numerics;

using UnityEngine;

namespace Sackrany.Actor.DefaultFeatures.BehaviourFeature.Modules
{
    public abstract class MovementBehaviourModule : Module
    {
        [Template] protected MovementBehaviour _template;
        [Dependency] protected StatHandlerModule _stats;
        [Dependency(true)] protected ConditionHandlerModule _conditions;
        protected Vector3 _currentMove;
        
        public bool WasMoved { get; protected set; }
        public bool IsGrounded { get; protected set; }
        public float LastMoveSpeed { get; private set; }
        public Vector3 DeltaPosition => delta;
        public float DeltaPositionMagnitude => deltaMagnitude;
        
        public Vector3 AdditionalVelocity;
        public GateModifiable<bool> IsSprinting;
        public Modifiable<float> SprintMultiplier;
        public SequentialRewriteVariable<Vector3> DirectionRewrite;

        Modifiable<float> _moveSpeed;
        
        bool CanMove => _conditions == null || _conditions.IsAllowed<CanMove>();
        
        protected override void OnAwake()
        {
            IsSprinting = false;
            DirectionRewrite = new SequentialRewriteVariable<Vector3>();
            AdditionalVelocity = Vector3.zero;
            SprintMultiplier = _template.sprintMultiplier;
        }
        protected override void OnStart()
        {
            _moveSpeed = _stats.GetStat<MoveSpeed>();
        }
        protected override void OnReset()
        {
            OnGrounded = null;
            SprintMultiplier.Clear();
            IsSprinting.Clear();
            DirectionRewrite.Clear();
        }
        
        public void Move(Vector3 direction)
        {
            if (!CanMove) return;
            LastMoveSpeed = _moveSpeed * (IsSprinting ? SprintMultiplier : 1f);
            _currentMove += DirectionRewrite.Calculate(direction.normalized) * LastMoveSpeed;
            WasMoved = true;
        }
        public void MoveRaw(Vector3 direction)
        {
            if (!CanMove) return;
            LastMoveSpeed = _moveSpeed * (IsSprinting ? SprintMultiplier : 1f);
            _currentMove += DirectionRewrite.Calculate(direction) * LastMoveSpeed;
            WasMoved = true;
        }
        public void MoveRelative(Vector3 direction)
        {
            if (!CanMove) return;
            LastMoveSpeed = _moveSpeed * (IsSprinting ? SprintMultiplier : 1f);
            _currentMove += DirectionRewrite.Calculate(Unit.transform.TransformDirection(direction.normalized)) * LastMoveSpeed;
            WasMoved = true;
        }

        Vector3 _lastPosition;
        protected Vector3 delta;
        protected float deltaMagnitude;
        protected void RecalculateDelta()
        {
            delta = Unit.transform.position - _lastPosition;
            deltaMagnitude = delta.magnitude;
            _lastPosition = Unit.transform.position;
        }

        public Action<float> OnGrounded;
    }

    public class CharacterController_MovementBehaviour : MovementBehaviourModule, IUpdateModule
    {
        [Dependency] CharacterController _characterController;
        bool _wasGrounded;
        public void OnUpdate(float deltaTime)
        {
            CurrentMove(deltaTime);
            RecalculateDelta();
        }
        void CurrentMove(float deltaTime)
        {
            float addVMgn = AdditionalVelocity.sqrMagnitude;
            if (addVMgn > float.Epsilon * 2f)
            {
                _currentMove += AdditionalVelocity;
                AdditionalVelocity -= AdditionalVelocity * deltaTime * (addVMgn + 1f);
                WasMoved = true;
            }
            
            IsGrounded = _characterController.isGrounded || _template.isFlying;
            if (!_wasGrounded && IsGrounded)
            {
                _wasGrounded = true;
                OnGrounded?.Invoke(deltaMagnitude);
            }
            _wasGrounded = IsGrounded;
            
            if (!WasMoved)
            {
                return;
            }
            _characterController.Move(_currentMove * deltaTime);
            _currentMove = Vector3.Lerp(_currentMove, Vector3.zero, deltaTime * 25f);
            if (_currentMove.sqrMagnitude <= float.Epsilon * 10) WasMoved = false;
        }
    }
    public class Rigidbody_MovementBehaviour : MovementBehaviourModule, IFixedUpdateModule
    {
        [Dependency] Rigidbody _rigidbody;
        bool _wasGrounded;
        public void OnFixedUpdate(float deltaTime)
        {
            CurrentMove(deltaTime);
            RecalculateDelta();
        }
        void CurrentMove(float deltaTime)
        {
            float addVMgn = AdditionalVelocity.sqrMagnitude;
            if (addVMgn > float.Epsilon * 2f)
            {
                _currentMove += AdditionalVelocity;
                AdditionalVelocity -= AdditionalVelocity * deltaTime * (addVMgn + 1f);
                WasMoved = true;
            }

            IsGrounded = true;//TODO_characterController.isGrounded || _template.isFlying;
            if (!_wasGrounded && IsGrounded)
            {
                _wasGrounded = true;
                OnGrounded?.Invoke(deltaMagnitude);
            }
            _wasGrounded = IsGrounded;
            
            if (!WasMoved)
            {
                return;
            }
            _rigidbody.AddForce(_currentMove * deltaTime);
            _currentMove = Vector3.Lerp(_currentMove, Vector3.zero, deltaTime * 25f);
            if (_currentMove.sqrMagnitude <= float.Epsilon * 10) WasMoved = false;
        }
    }
    public class Transform_MovementBehaviour : MovementBehaviourModule, IUpdateModule
    {
        bool _wasGrounded;
        public void OnUpdate(float deltaTime)
        {
            CurrentMove(deltaTime);
            RecalculateDelta();
        }
        void CurrentMove(float deltaTime)
        {
            float addVMgn = AdditionalVelocity.sqrMagnitude;
            if (addVMgn > float.Epsilon * 2f)
            {
                _currentMove += AdditionalVelocity;
                AdditionalVelocity -= AdditionalVelocity * deltaTime * (addVMgn + 1f);
                WasMoved = true;
            }

            IsGrounded = true;
            if (!_wasGrounded && IsGrounded)
            {
                _wasGrounded = true;
                OnGrounded?.Invoke(deltaMagnitude);
            }
            _wasGrounded = IsGrounded;
            
            if (!WasMoved)
            {
                return;
            }
            Unit.transform.position += _currentMove * deltaTime;
            _currentMove = Vector3.Lerp(_currentMove, Vector3.zero, deltaTime * 25f);
            if (_currentMove.sqrMagnitude <= float.Epsilon * 10) WasMoved = false;
        }
    }

    [Serializable]
    public struct MovementBehaviour : ModuleTemplate
    {
        public ControllerType controllerType;
        public bool isFlying;
        public float sprintMultiplier;
        public int GetId() => controllerType switch
        {
            ControllerType.CharacterController => ModuleRegistry.GetId<CharacterController_MovementBehaviour>(),
            ControllerType.Transform => ModuleRegistry.GetId<Transform_MovementBehaviour>(),
            _ => ModuleRegistry.GetId<Rigidbody_MovementBehaviour>(),
        };
        public Module GetInstance() => controllerType switch
        {
            ControllerType.CharacterController => new CharacterController_MovementBehaviour(),
            ControllerType.Transform => new Transform_MovementBehaviour(),
            _ => new Rigidbody_MovementBehaviour(),
        };

        public enum ControllerType
        {
            CharacterController,
            Rigidbody,
            Transform,
        }
    }

    public class MovementIsoCorrectorModule : Module
    {
        [Template] MovementIsoCorrector _template;
        [Dependency] MovementBehaviourModule _movementModule;
        protected override void OnStart()
        {
            if (_template.correctOnlyVerticalSpeed)
                _movementModule.DirectionRewrite.Subscribe((dir) 
                    => new Vector2(dir.x, dir.y * 0.816f)
                    );
            else                
                _movementModule.DirectionRewrite.Subscribe((dir) 
                    => new Vector2(dir.x, dir.y * 0.6123f)
                        .normalized
                        .Multiply(y: 0.816f)
                );
        }
    }
    [Serializable]
    public class MovementIsoCorrector : ModuleTemplate<MovementIsoCorrectorModule>
    {
        public bool correctOnlyVerticalSpeed;
    }
}