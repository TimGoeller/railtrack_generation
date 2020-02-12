using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class MeshBase
{
    public LinkedList<Vector3> BaseVertices;
    public MeshExtender Extender;

    public virtual ConstructedProceduralMesh ConstructMesh()
    {
        return Extender.ConstructMesh(this);
    }
}
