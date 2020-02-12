using UnityEngine;
using UnityEditor;

public class StraightExtensionMode : MeshExtensionMode
{
    public float length;

    public StraightExtensionMode(float length)
    {
        this.length = length;
    }
}