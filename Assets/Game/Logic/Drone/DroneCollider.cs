using System;

using UnityEngine;

namespace Game.Logic.Drone
{
    [RequireComponent(typeof(Collider2D))]
    public class DroneCollider : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D other)
        {
            OnHit?.Invoke(other.relativeVelocity);
        }

        public event Action<Vector2> OnHit;
    }
}