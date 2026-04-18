using System;

using Sackrany.Actor.DefaultFeatures.BehaviourFeature.Modules;
using Sackrany.Actor.Modules;
using Sackrany.ActorAI.PriorityBehaviour.Base;
using Sackrany.Extensions;

using UnityEngine;

namespace Sackrany.ActorAI.PriorityBehaviour.DefaultBehaviours
{
    [Serializable]
    public struct PatrolBehaviourData : IAiBehaviourTemplate<PatrolBehaviour>
    {
        public float radius;
        public float waitTimeMin;
        public float waitTimeMax;
        public float arrivalRadius;
    }

    public class PatrolBehaviour : AiBehaviourBase
    {
        [BehaviourTemplate] PatrolBehaviourData  _data;
        [Dependency] MovementBehaviourModule _movement;

        Vector3 _origin;
        Vector3 _destination;
        float   _waitTimer;
        bool    _waiting;

        public override void OnActivate()
        {
            if (_origin == default)
                _origin = Unit.transform.position.With(z: 0);

            if (_destination == default)
                PickNewDestination();
        }

        public override void OnTick(float deltaTime)
        {
            if (_waiting)
            {
                _waitTimer -= deltaTime;
                if (_waitTimer <= 0f)
                {
                    _waiting = false;
                    PickNewDestination();
                }
                return;
            }

            var toDestination = _destination - Unit.transform.position;
            toDestination.z   = 0f;

            if (toDestination.magnitude <= _data.arrivalRadius)
            {
                _waiting   = true;
                _waitTimer = UnityEngine.Random.Range(_data.waitTimeMin, _data.waitTimeMax);
                return;
            }

            _movement.Move(toDestination);
        }

        public override void OnDeactivate()
        {
            
        }

        void PickNewDestination()
        {
            var randomOffset = UnityEngine.Random.insideUnitCircle * _data.radius;
            _destination     = _origin + new Vector3(randomOffset.x, randomOffset.y, 0);
        }
    }
}