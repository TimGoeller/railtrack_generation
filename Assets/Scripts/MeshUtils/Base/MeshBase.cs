using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class MeshBase
{
    public List<Vector3> BaseVertices;
    public MeshExtender Extender;

    public ConstructedProceduralMesh ConstructMesh()
    {
        Extender.ConstructMesh(this);
    }
}
