using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadBase : MeshBase
{
    public float Width, Height;

    public override ConstructedProceduralMesh ConstructMesh()
    {
        BaseVertices = new LinkedList<Vector3>();
        BaseVertices.AddLast(new Vector3(0, 0));
        BaseVertices.AddLast(new Vector3(0, 0, Height));
        BaseVertices.AddLast(new Vector3(Width, 0, Height));
        BaseVertices.AddLast(new Vector3(Width, 0, 0));
        
        return base.ConstructMesh();
    }
}
