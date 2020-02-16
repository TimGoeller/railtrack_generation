using UnityEngine;
using UnityEditor;

public class StraightExtensionMode : MeshExtensionMode
{
    public float length;

    public StraightExtensionMode(float length)
    {
        this.length = length;
    }

    public override Vector3 ExtendPoint(Vector3 originalPoint, Vector3 center)
    {
        return center;
    }
}