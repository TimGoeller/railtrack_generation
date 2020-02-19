using Boo.Lang;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;

public class SubmeshConstructor
{
    public Vector3[] Vertices = new Vector3[] { };
    public int[][] SubmeshTriangles = new int[][] { };
    public Vector2[] UVs = new Vector2[] { };

    public SubmeshConstructor(List<ConstructedProceduralMesh> constructedProceduralMeshes)
    {
        SubmeshTriangles = new int[constructedProceduralMeshes.Count][];

        int index = 0;
        foreach (ConstructedProceduralMesh procMesh in constructedProceduralMeshes)
        {
            int previousVerticesCount = Vertices?.Length ?? 0;
            Vertices = Vertices.Concat(procMesh.Vertices).ToArray();

            for (int i = 0; i < procMesh.Triangles.Length; i++)
            {
                procMesh.Triangles[i] += previousVerticesCount;
            }

            SubmeshTriangles[index] = procMesh.Triangles;

            UVs = UVs.Concat(procMesh.UVs).ToArray();

            index++;
        }
    }
}