using System;

using ModifiableVariable;
using ModifiableVariable.Stages.StageFactory;

using R3;

using Sackrany.Actor.Managers;
using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;

using UnityEngine;

namespace Game.Logic.Drone.Modules
{
    public class DroneFlyModule : Module, IFixedUpdateModule
    {
        public Modifiable<float> LinearAcceleration { get; private set; } = 0;
        public Modifiable<float> AngularAcceleration { get; private set; } = 0;
        public Modifiable<float> AngularAdditional { get; private set; } = 0;

        public Modifiable<float> LinearDamping { get; private set; } = 0;
        public Modifiable<float> AngularDamping { get; private set; } = 0;

        public PositionModifiable<Vector2> MoveDirection { get; private set; } = Vector2.zero;

        public ReactiveProperty<Vector2> LinearVelocity { get; private set; }
        public ReactiveProperty<float> AngularVelocity { get; private set; }

        [Template] DroneFly _template;
        [Dependency] Rigidbody2D _rb;

        protected override void OnStart()
        {
            LinearVelocity = new ReactiveProperty<Vector2>(Vector2.zero);
            AngularVelocity = new ReactiveProperty<float>(0);

            LinearAcceleration.Add(() => _template.LinearAcceleration);
            AngularAcceleration.Add(() => _template.AngularAcceleration);

            LinearDamping.Add(() => _template.LinearDamping);
            AngularDamping.Add(() => _template.AngularDamping);

            LinearDamping.Add(() => 1f / UnitTimeFlowManager.TimeFlow, General.Multiply);
            AngularDamping.Add(() => 1f / UnitTimeFlowManager.TimeFlow, General.Multiply);
        }

        protected override void OnReset()
        {
            LinearVelocity.Dispose();
            AngularVelocity.Dispose();

            LinearAcceleration.Clear();
            AngularAcceleration.Clear();
            AngularAdditional.Clear();
            
            LinearDamping.Clear();
            AngularDamping.Clear();
            
            MoveDirection.Clear();
        }

        public void OnFixedUpdate(float deltaTime)
        {
            _rb.linearDamping = LinearDamping.GetValue();
            _rb.angularDamping = AngularDamping.GetValue();

            Vector2 dir = MoveDirection.GetValue();

            if (dir.magnitude > Mathf.Epsilon)
            {
                _rb.AddForce(dir * LinearAcceleration.GetValue(), ForceMode2D.Force);

                float error = Vector2.SignedAngle(_rb.transform.right, dir);
                _rb.AddTorque(error * AngularAcceleration.GetValue() + AngularAdditional, ForceMode2D.Force);
            }

            LinearVelocity.Value = _rb.linearVelocity;
            AngularVelocity.Value = _rb.angularVelocity;
        }
    }

    [Serializable]
    public struct DroneFly : ModuleTemplate<DroneFlyModule>
    {
        public float LinearAcceleration;
        public float AngularAcceleration;

        public float LinearDamping;
        public float AngularDamping;
    }
}