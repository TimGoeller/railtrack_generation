using UnityEngine;
using UnityEditor;

public class RaycastUtil : ScriptableObject
{
    public struct XZRaycast
    {
        public bool Hit;
        public Vector3 HitPoint;

        public XZRaycast(bool hit, Vector3 hitPoint)
        {
            Hit = hit;
            HitPoint = hitPoint;
        }
    }

    public static XZRaycast VectorToXZPlane(Vector3 vectorToConvert)
    {
        var plane = new Plane(Vector3.up, Vector3.zero);
        var ray = HandleUtility.GUIPointToWorldRay(vectorToConvert);

        float distance;

        if (plane.Raycast(ray, out distance))
        {
            return new XZRaycast(true, ray.GetPoint(distance));
        }
        return new XZRaycast(false, Vector3.zero);
    }
}