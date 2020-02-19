using Boo.Lang;
using UnityEngine;
using UnityEditor;

public class RailConstructor : MeshConstructor
{
    public List<Vector3> RailSegments;
    public List<Vector3> RailSegmentVerticalNormals;
    public float RailWidth;
    public float RailHeight;
    public float RailMidPercentage;

    public override ConstructedProceduralMesh ConstructMesh()
    {
        ConstructedProceduralMesh mesh = new ConstructedProceduralMesh();

        int segmentIndex = 0;
        foreach (Vector3 segment in RailSegments)
        {
            if (segmentIndex == RailSegments.Count - 1) break;

            Vector3 segmentVerticalNormal = RailSegmentVerticalNormals[segmentIndex];
            Vector3 nextSegment = RailSegments[segmentIndex + 1];
            Vector3 nextSegmentVerticalNormals = RailSegmentVerticalNormals[segmentIndex + 1];

            Quad railBase = new Quad()
            {
                LowerLeft = segment - segmentVerticalNormal * (RailWidth / 2),
                UpperLeft = nextSegment - nextSegmentVerticalNormals * (RailWidth / 2),
                UpperRight = nextSegment + nextSegmentVerticalNormals * (RailWidth / 2),
                LowerRight = segment + segmentVerticalNormal * (RailWidth / 2)
            };

            Quad railBaseSmall = new Quad()
            {
                LowerLeft = segment - segmentVerticalNormal * (0.5f * RailWidth / 2),
                UpperLeft = nextSegment - nextSegmentVerticalNormals * (0.5f * RailWidth / 2),
                UpperRight = nextSegment + nextSegmentVerticalNormals * (0.5f * RailWidth / 2),
                LowerRight = segment + segmentVerticalNormal * (0.5f * RailWidth / 2)
            };

            float midPartHeight = RailHeight * RailMidPercentage;
            float outerPartHeight = (RailHeight - midPartHeight) / 2;

            #region Lowest part
            float lowestPartBottomHeight = outerPartHeight * 0.5f;
            Quad railLowestPartBottomTop = railBase + new Vector3(0, lowestPartBottomHeight, 0);
            Quad railLowestPartTopTop = railBaseSmall + new Vector3(0, outerPartHeight, 0);

            mesh.AddMesh(ConstructCube(railBase, railLowestPartBottomTop, false, true));

            float uvOffsetStart = 0.5f * (RailWidth / 2);
            float uvOffsetEnd = uvOffsetStart + (RailWidth / 2);

            mesh.AddMesh(ConstructFaceBetween(
                new Quad(railLowestPartBottomTop.LowerLeft, railLowestPartTopTop.LowerLeft, railLowestPartTopTop.LowerRight, railLowestPartBottomTop.LowerRight),
                new Vector2[] {new Vector2(0, 0), new Vector2(uvOffsetStart, lowestPartBottomHeight), new Vector2(uvOffsetEnd, lowestPartBottomHeight), new Vector2(RailWidth, 0) }));
            mesh.AddMesh(ConstructFaceBetween(
                new Quad(railLowestPartBottomTop.LowerRight, railLowestPartTopTop.LowerRight, railLowestPartTopTop.UpperRight, railLowestPartBottomTop.UpperRight)));
            mesh.AddMesh(ConstructFaceBetween(
                new Quad(railLowestPartBottomTop.UpperRight, railLowestPartTopTop.UpperRight, railLowestPartTopTop.UpperLeft, railLowestPartBottomTop.UpperLeft),
                new Vector2[] { new Vector2(0, 0), new Vector2(uvOffsetStart, lowestPartBottomHeight), new Vector2(uvOffsetEnd, lowestPartBottomHeight), new Vector2(RailWidth, 0) }));
            mesh.AddMesh(ConstructFaceBetween(
                new Quad(railLowestPartBottomTop.UpperLeft, railLowestPartTopTop.UpperLeft, railLowestPartTopTop.LowerLeft, railLowestPartBottomTop.LowerLeft)));
            #endregion

            #region Middle part
            Quad middlePartTop = railBaseSmall + new Vector3(0, (midPartHeight + outerPartHeight), 0);

            mesh.AddMesh(ConstructCube(railLowestPartTopTop, middlePartTop, false, true));
            #endregion

            #region Top part
            Quad railHighestPartBottomTop = railBase + new Vector3(0, (midPartHeight + outerPartHeight) + (outerPartHeight - lowestPartBottomHeight), 0);
            Quad railHighestPartTopTop = railBase + new Vector3(0, RailHeight, 0);

            mesh.AddMesh(ConstructCube(middlePartTop, railHighestPartBottomTop, false, false));

            mesh.AddMesh(ConstructFaceBetween(
                new Quad(middlePartTop.LowerLeft, railHighestPartBottomTop.LowerLeft, railHighestPartBottomTop.LowerRight, middlePartTop.LowerRight),
                new Vector2[] { new Vector2(uvOffsetStart, 0), new Vector2(0, lowestPartBottomHeight), new Vector2(RailWidth, lowestPartBottomHeight), new Vector2(uvOffsetEnd, 0) }));
            mesh.AddMesh(ConstructFaceBetween(
                new Quad(middlePartTop.LowerRight, railHighestPartBottomTop.LowerRight, railHighestPartBottomTop.UpperRight, middlePartTop.UpperRight)));
            mesh.AddMesh(ConstructFaceBetween(
                new Quad(middlePartTop.UpperRight, railHighestPartBottomTop.UpperRight, railHighestPartBottomTop.UpperLeft, middlePartTop.UpperLeft),
                new Vector2[] { new Vector2(0, 0), new Vector2(uvOffsetStart, lowestPartBottomHeight), new Vector2(uvOffsetEnd, lowestPartBottomHeight), new Vector2(RailWidth, 0) }));
            mesh.AddMesh(ConstructFaceBetween(
                new Quad(middlePartTop.UpperLeft, railHighestPartBottomTop.UpperLeft, railHighestPartBottomTop.LowerLeft, middlePartTop.LowerLeft)));


            mesh.AddMesh(ConstructCube(railHighestPartBottomTop, railHighestPartTopTop, true, false));
            #endregion

            Quad railTop = railBaseSmall + new Vector3(0, RailHeight, 0);

            //mesh.AddMesh(ConstructCube(railBase, railTop, false, true));

            segmentIndex++;
        }

        

        return mesh;
    }
}