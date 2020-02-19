using System;
using System.Linq;
using System.Net.Configuration;
using Boo.Lang;
using UnityEngine;
using UnityEditor;

public abstract class MeshConstructor
{
    public abstract ConstructedProceduralMesh ConstructMesh();

    protected ConstructedProceduralMesh ConstructFaceBetween(Quad face, Vector2[] uv = null)
    {
        ConstructedProceduralMesh mesh = new ConstructedProceduralMesh();

        float faceHeight = (face.UpperLeft - face.LowerLeft).magnitude;
        float faceWidth = (face.LowerRight - face.LowerLeft).magnitude;

        mesh.Vertices = new[] { face.LowerLeft, face.UpperLeft, face.UpperRight, face.LowerRight };
        mesh.Triangles = new[] { 0, 1, 2, 3, 0, 2 };

        if (uv == null)
        {
            mesh.UVs = new[]
            {
                new Vector2(0, 0), new Vector2(0, faceHeight), new Vector2(faceWidth, faceHeight), new Vector2(faceWidth, 0)
            };
        }
        else
        {
            mesh.UVs = uv;
        }
       

        return mesh;
    }

    protected ConstructedProceduralMesh ConstructCube(Quad bottom, Quad upper, bool topFace = true, bool bottomFace = true)
    {
        ConstructedProceduralMesh mesh = new ConstructedProceduralMesh();

        mesh.AddMesh(ConstructFaceBetween(new Quad(bottom.LowerLeft, upper.LowerLeft, upper.LowerRight, bottom.LowerRight)));
        mesh.AddMesh(ConstructFaceBetween(new Quad(bottom.LowerRight, upper.LowerRight, upper.UpperRight, bottom.UpperRight)));
        mesh.AddMesh(ConstructFaceBetween(new Quad(bottom.UpperRight, upper.UpperRight, upper.UpperLeft, bottom.UpperLeft)));
        mesh.AddMesh(ConstructFaceBetween(new Quad(bottom.UpperLeft, upper.UpperLeft, upper.LowerLeft, bottom.LowerLeft)));

        if (topFace)
        {
            mesh.AddMesh(ConstructFaceBetween(upper));
        }

        if (bottomFace)
        {
            mesh.AddMesh(ConstructFaceBetween(new Quad(bottom.UpperLeft, bottom.LowerLeft, bottom.LowerRight, bottom.UpperRight)));
        }

        return mesh;
    }

}