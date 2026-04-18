using UnityEngine;

namespace Sackrany.Extensions
{
    public static class CameraExtensions
    {
        public static bool IsCameraRayHit(this Camera camera, out RaycastHit hit, float distance = 100f, int mask = 0)
        {
            var ray = new Ray(camera.transform.position, camera.transform.forward);
            return Physics.Raycast(ray, out hit, distance, mask);
        }
    }
}