using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlatMeshExtender : MeshExtender
{
    public float Size; //TODO Mesh Extension Mode

    public override ConstructedProceduralMesh ConstructMesh(MeshBase meshBase)
    {
        ConstructedProceduralMesh mesh = new ConstructedProceduralMesh();

        LinkedListNode<Vector3> currentNode = meshBase.BaseVertices.First;
        while (currentNode != null)
        {
            Vector3[] vertices = new Vector3[4];
            vertices[0] = currentNode.Value;
            vertices[1] = currentNode.Next?.Value ?? meshBase.BaseVertices.First.Value;

            vertices[2] = vertices[0] + new Vector3(0, Size, 0);
            vertices[3] = vertices[1] + new Vector3(0, Size, 0);

            int[] triangles = new int[6];
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 1;
            triangles[4] = 3;
            triangles[5] = 2;

            Vector2[] uvs = new Vector2[vertices.Length];

            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(1, 0);
            uvs[2] = new Vector2(0, 1);
            uvs[3] = new Vector2(1, 1);

            mesh.AddMeshData(vertices, triangles, uvs);

            currentNode = currentNode.Next;
        }

        return mesh;
    }
}