
using UnityEngine;

namespace Application.Utils
{
    public static class Vector3Extension
    {
        public static float DistanceInSphereSurface(this Vector3 pointA, Vector3 pointB, float sphereRadius)
        {
            Vector3 fromDir = pointA.normalized;
            Vector3 toDir = pointB.normalized;
            float angleRadians = Mathf.Acos(Mathf.Clamp(Vector3.Dot(fromDir, toDir), NumberConstants.MinusOne, NumberConstants.One));
            return angleRadians * sphereRadius;
        }

    }
}
