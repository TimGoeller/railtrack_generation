using System.Linq;
using System.Runtime.InteropServices;
using Boo.Lang;
using UnityEngine;
using UnityEditor;

public class ConstructedProceduralMesh
{
    public Vector3[] Vertices = new Vector3[]{};
    public int[] Triangles = new int[] { };
    public Vector2[] UVs = new Vector2[] { };

    private void AddMeshData(Vector3[] newVertices, int[] newTriangles, Vector2[] newUvs)
    {
        int previousVerticesCount = Vertices?.Length ?? 0;
        Vertices = Vertices.Concat(newVertices).ToArray();

        for (int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[i] += previousVerticesCount;
        }

        Triangles = Triangles.Concat(newTriangles).ToArray();
        UVs = UVs.Concat(newUvs).ToArray();
    }

    public ConstructedProceduralMesh AddMesh(ConstructedProceduralMesh mesh)
    {
        AddMeshData(mesh.Vertices, mesh.Triangles, mesh.UVs);
        return this;
    }
}