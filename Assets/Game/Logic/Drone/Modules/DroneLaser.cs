using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Extensions;

using UnityEngine;

namespace Game.Logic.Drone.Modules
{
    public class DroneLaserModule : Module, IFixedUpdateModule
    {
        [Template] DroneLaser _template;
        [Dependency] LineRenderer _line;
        [Dependency] DroneMissleModule _missle;
        float _maxDistance = 500f;
        int _layers;
        
        protected override void OnStart()
        {
            _layers = LayerMask.GetMask("Obstacle", "Enemy");    
        }
        
        public void OnFixedUpdate(float deltaTime)
        {
            Vector2 origin = _line.transform.position;
            Vector2 dir = _missle.AimDirection;

            var hit = Physics2D.Raycast(origin, dir, _maxDistance, _layers);

            Vector3 endPoint;

            if (hit.collider != null)
            {
                endPoint = hit.point;
            }
            else
            {
                endPoint = origin + dir * _maxDistance;
            }

            _line.positionCount = 2;
            _line.SetPosition(0, origin);
            _line.SetPosition(1, endPoint);
            _template.Point.transform.position = endPoint.With(z: -5);
        }
    }

    [Serializable]
    public struct DroneLaser : ModuleTemplate<DroneLaserModule>
    {
        public Transform Point;
    }
}