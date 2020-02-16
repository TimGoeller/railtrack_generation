using UnityEngine;
using UnityEditor;

public abstract class MeshExtensionMode
{
    public abstract Vector3 ExtendPoint(Vector3 originalPoint, Vector3 center);
}