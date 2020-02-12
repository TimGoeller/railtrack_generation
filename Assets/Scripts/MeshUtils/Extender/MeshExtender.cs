using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

public abstract class MeshExtender
{
    public MeshExtensionMode ExtensionMode;

    public abstract ConstructedProceduralMesh ConstructMesh(MeshBase meshBase);
}