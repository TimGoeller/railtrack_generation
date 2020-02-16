using UnityEngine;
using UnityEditor;

public class RailSegmentConstructor : MeshConstructor
{
    public Quad Base;
    public float Height, LengthCutoutPercentage, HeightCutoutPercentage;

    public override ConstructedProceduralMesh ConstructMesh()
    {
        ConstructedProceduralMesh mesh = new ConstructedProceduralMesh();

        float midHeight = Height - (Height * HeightCutoutPercentage);

        Vector3 horizontalTop = ((Base.UpperLeft - Base.LowerLeft) / 2) * LengthCutoutPercentage;

        Quad mid = Base + new Vector3(0, midHeight, 0);
        Quad top = new Quad()
        {
            LowerLeft = Base.LowerLeft + horizontalTop,
            UpperLeft = Base.UpperLeft - horizontalTop,
            UpperRight = Base.UpperRight - horizontalTop,
            LowerRight = Base.LowerRight + horizontalTop
        } + new Vector3(0, Height, 0);

        mesh.AddMesh(ConstructCube(Base, mid, false, true));
        mesh.AddMesh(ConstructCube(mid, top, true, false));

        return mesh;
    }
}