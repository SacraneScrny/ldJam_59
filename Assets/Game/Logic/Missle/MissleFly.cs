using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Utils;
using Sackrany.Utils.Pool.Extensions;

using UnityEngine;

namespace Game.Logic.Missle
{
    [UpdateOrder(Order.AfterAll)]
    public class MissleFlyModule : Module, IFixedUpdateModule
    {
        [Template] MissleFly _template;

        public float Speed { get; private set; }
        public float Distance { get; private set; }
        
        float _time;
        protected override void OnReset()
        {
            OnFinished = null;
            Distance = 0;
            Speed = 0;
            _time = 0;
        }
        public void OnFixedUpdate(float deltaTime)
        {
            _time += deltaTime;
            Speed += deltaTime * _template.Accel;
            Speed = Mathf.Min(Speed, _template.MaxSpeed);
            Unit.transform.position += Unit.transform.right * deltaTime * Speed;
            Distance += deltaTime * Speed;
            
            if (_time >= _template.TimeLive)
            {
                StopWorking();
            }
        }

        public void StopWorking()
        {
            OnFinished?.Invoke();
            Unit.StopWork();
            DeferredExecution.AfterDelay(1f, () => Unit.RELEASE());
        }

        public event Action OnFinished;
    }

    [Serializable]
    public struct MissleFly : ModuleTemplate<MissleFlyModule>
    {
        [HashKey] public float Accel;
        [HashKey] public float MaxSpeed;
        [HashKey] public float TimeLive;
    } 
}