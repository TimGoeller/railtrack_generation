using System.Linq;
using System.Runtime.InteropServices;
using Boo.Lang;
using UnityEngine;
using UnityEditor;

public class ConstructedProceduralMesh
{
    public Vector3[] vertices = new Vector3[]{};
    public int[] triangles = new int[] { };
    public Vector2[] uvs = new Vector2[] { };

    public void AddMeshData(Vector3[] newVertices, int[] newTriangles, Vector2[] newUvs)
    {
        int previousVerticesCount = vertices?.Length ?? 0;
        vertices = vertices.Concat(newVertices).ToArray();

        for (int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[i] += previousVerticesCount;
        }

        triangles = triangles.Concat(newTriangles).ToArray();
        uvs = uvs.Concat(newUvs).ToArray();
    }
}