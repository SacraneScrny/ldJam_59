using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Utils.Pool.Extensions;

using UnityEngine;

namespace Game.Logic.Missle
{
    [UpdateOrder(Order.AfterAll)]
    public class MissleFlyModule : Module, IFixedUpdateModule
    {
        [Template] MissleFly _template;

        float _speed;
        float _time;
        protected override void OnReset()
        {
            _speed = 0;
            _time = 0;
        }
        public void OnFixedUpdate(float deltaTime)
        {
            _time += deltaTime;
            _speed += deltaTime * _template.Accel;
            _speed = Mathf.Min(_speed, _template.MaxSpeed);
            Unit.transform.position += Unit.transform.right * deltaTime * _speed;
            
            if (_time >= _template.TimeLive)
                Unit.RELEASE();
        }
    }

    [Serializable]
    public struct MissleFly : ModuleTemplate<MissleFlyModule>
    {
        [HashKey] public float Accel;
        [HashKey] public float MaxSpeed;
        [HashKey] public float TimeLive;
    } 
}