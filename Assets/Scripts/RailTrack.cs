using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class RailTrack : MonoBehaviour
{
    #region Attributes
    public float trackTotalWidth = 2;
    public float trackWidth = 0.1f;
    public float trackHeight = 0.1f;
    public float segmentWidth = 0.5f;
    public float segmentHeight = 0.25f;
    public int segmentCount = 10;
    public float spaceBetweenSegments = 0.5f;
    public float trackOffsetPercentage = 0.15f;
    public float segmentCutoutWidthPercentage = 0.75f;
    public float segmentCutoutHeightPercentage = 0.75f;

    private Vector3 segmentBaseLowerLeft, segmentBaseUpperLeft, segmentBaseUpperRight, segmentBaseLowerRight,
        segmentMiddleLowerLeft, segmentMiddleUpperLeft, segmentMiddleUpperRight, segmentMiddleLowerRight,
        segmentTopLowerLeft, segmentTopUpperLeft, segmentTopUpperRight, segmentTopLowerRight;

    private Vector3 leftTrackStart, leftTrackEnd, rightTrackStart, rightTrackEnd;


    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    #endregion

    public void OnDrawGizmos()
    {
        for(int i = 0; i < segmentCount; i++)
        {
            Vector3 segmentOffset = new Vector3(0, 0, i * segmentWidth + i * spaceBetweenSegments);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(segmentBaseLowerLeft + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentBaseUpperLeft + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentBaseUpperRight + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentBaseLowerRight + segmentOffset, 0.1f);

            Gizmos.DrawSphere(segmentMiddleLowerLeft + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentMiddleUpperLeft + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentMiddleUpperRight + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentMiddleLowerRight + segmentOffset, 0.1f);

            Gizmos.DrawSphere(segmentTopLowerLeft + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentTopUpperLeft + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentTopUpperRight + segmentOffset, 0.1f);
            Gizmos.DrawSphere(segmentTopLowerRight + segmentOffset, 0.1f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftTrackStart, 0.1f);
        Gizmos.DrawSphere(leftTrackEnd, 0.1f);
        Gizmos.DrawSphere(rightTrackStart, 0.1f);
        Gizmos.DrawSphere(rightTrackEnd, 0.1f);
    }

    public void CreateMesh()
    {
        #region Initialization
        if (!meshFilter)
            meshFilter = GetComponent<MeshFilter>();

        if (!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();

        mesh = meshFilter.sharedMesh;

        if (!mesh)
        {
            mesh = new Mesh();
            mesh.name = "Track";
        }

        segmentBaseLowerLeft = Vector3.zero;
        segmentBaseUpperLeft = segmentBaseLowerLeft + new Vector3(0, 0, segmentWidth);
        segmentBaseUpperRight = segmentBaseUpperLeft + new Vector3(trackTotalWidth, 0, 0);
        segmentBaseLowerRight = segmentBaseLowerLeft + new Vector3(trackTotalWidth, 0, 0);

        Vector3 segmentBaseHeightOffset = new Vector3(0, segmentHeight * segmentCutoutHeightPercentage, 0);

        segmentMiddleLowerLeft = segmentBaseLowerLeft + segmentBaseHeightOffset;
        segmentMiddleUpperLeft = segmentBaseUpperLeft + segmentBaseHeightOffset;
        segmentMiddleUpperRight = segmentBaseUpperRight + segmentBaseHeightOffset;
        segmentMiddleLowerRight = segmentBaseLowerRight + segmentBaseHeightOffset;

        Vector3 segmentHeightOffset = new Vector3(0, segmentHeight, 0);
        segmentTopLowerLeft = segmentBaseLowerLeft + segmentHeightOffset + new Vector3(0, 0, (segmentWidth / 2) * segmentCutoutWidthPercentage);
        segmentTopUpperLeft = segmentBaseUpperLeft + segmentHeightOffset - new Vector3(0, 0, (segmentWidth / 2) * segmentCutoutWidthPercentage);
        segmentTopUpperRight = segmentBaseUpperRight + segmentHeightOffset - new Vector3(0, 0, (segmentWidth / 2) * segmentCutoutWidthPercentage);
        segmentTopLowerRight = segmentBaseLowerRight + segmentHeightOffset + new Vector3(0, 0, (segmentWidth / 2) * segmentCutoutWidthPercentage);

        Vector3 endSegmentOffset = new Vector3(0, 0, (segmentCount - 1) * segmentWidth + (segmentCount - 1) * spaceBetweenSegments);
        leftTrackStart = segmentTopLowerLeft + new Vector3(trackTotalWidth * trackOffsetPercentage, 0, 0);
        rightTrackStart = segmentTopLowerRight - new Vector3(trackTotalWidth * trackOffsetPercentage, 0, 0);

        leftTrackEnd = segmentTopUpperLeft + endSegmentOffset + new Vector3(trackTotalWidth * trackOffsetPercentage, 0, 0);
        rightTrackEnd = segmentTopUpperRight + endSegmentOffset - new Vector3(trackTotalWidth * trackOffsetPercentage, 0, 0);

        #endregion

        const int segmentVerticeCount = 36;
        const int trackVerticeCount = 20;
        Vector3[] vertices = new Vector3[segmentCount * segmentVerticeCount + trackVerticeCount];
        int[][] submeshTriangles = new int[segmentCount + 2][];

        List<Vector2> uvs = new List<Vector2>();

        #region Tracks
        vertices[0] = leftTrackStart;
        vertices[1] = leftTrackEnd;
        vertices[2] = leftTrackEnd + new Vector3(trackWidth, 0 , 0);
        vertices[3] = leftTrackStart + new Vector3(trackWidth, 0, 0);

        vertices[4] = vertices[0] + new Vector3(0, trackHeight, 0);
        vertices[5] = vertices[1] + new Vector3(0, trackHeight, 0);
        vertices[6] = vertices[2] + new Vector3(0, trackHeight, 0);
        vertices[7] = vertices[3] + new Vector3(0, trackHeight, 0);

        vertices[8] = vertices[4];
        vertices[9] = vertices[5];

        Vector3 rightTrackOffset = rightTrackStart - leftTrackStart - new Vector3(trackWidth, 0 , 0);

        vertices[10] = vertices[0] + rightTrackOffset;
        vertices[11] = vertices[1] + rightTrackOffset;
        vertices[12] = vertices[2] + rightTrackOffset;
        vertices[13] = vertices[3] + rightTrackOffset;

        vertices[14] = vertices[4] + rightTrackOffset;
        vertices[15] = vertices[5] + rightTrackOffset;
        vertices[16] = vertices[6] + rightTrackOffset;
        vertices[17] = vertices[7] + rightTrackOffset;

        vertices[18] = vertices[8] + rightTrackOffset;
        vertices[19] = vertices[9] + rightTrackOffset;

        submeshTriangles[0] = new int[30];
        submeshTriangles[1] = new int[30];

        submeshTriangles[0][0] = 0;
        submeshTriangles[0][1] = 7;
        submeshTriangles[0][2] = 3;

        submeshTriangles[0][3] = 0;
        submeshTriangles[0][4] = 4;
        submeshTriangles[0][5] = 7;

        submeshTriangles[0][6] = 0;
        submeshTriangles[0][7] = 1;
        submeshTriangles[0][8] = 4;

        submeshTriangles[0][9] = 4;
        submeshTriangles[0][10] = 1;
        submeshTriangles[0][11] = 5;

        submeshTriangles[0][12] = 2;
        submeshTriangles[0][13] = 5;
        submeshTriangles[0][14] = 1;

        submeshTriangles[0][15] = 6;
        submeshTriangles[0][16] = 5;
        submeshTriangles[0][17] = 2;

        submeshTriangles[0][18] = 2;
        submeshTriangles[0][19] = 7;
        submeshTriangles[0][20] = 6;

        submeshTriangles[0][21] = 2;
        submeshTriangles[0][22] = 3;
        submeshTriangles[0][23] = 7;

        submeshTriangles[0][24] = 7;
        submeshTriangles[0][25] = 8;
        submeshTriangles[0][26] = 6;

        submeshTriangles[0][27] = 6;
        submeshTriangles[0][28] = 4;
        submeshTriangles[0][29] = 9;



        submeshTriangles[1][0] = 10;
        submeshTriangles[1][1] = 17;
        submeshTriangles[1][2] = 13;

        submeshTriangles[1][3] = 10;
        submeshTriangles[1][4] = 14;
        submeshTriangles[1][5] = 17;

        submeshTriangles[1][6] = 10;
        submeshTriangles[1][7] = 11;
        submeshTriangles[1][8] = 14;

        submeshTriangles[1][9] = 14;
        submeshTriangles[1][10] = 11;
        submeshTriangles[1][11] = 15;

        submeshTriangles[1][12] = 12;
        submeshTriangles[1][13] = 15;
        submeshTriangles[1][14] = 11;

        submeshTriangles[1][15] = 16;
        submeshTriangles[1][16] = 15;
        submeshTriangles[1][17] = 12;

        submeshTriangles[1][18] = 12;
        submeshTriangles[1][19] = 17;
        submeshTriangles[1][20] = 16;

        submeshTriangles[1][21] = 12;
        submeshTriangles[1][22] = 13;
        submeshTriangles[1][23] = 17;

        submeshTriangles[1][24] = 17;
        submeshTriangles[1][25] = 18;
        submeshTriangles[1][26] = 16;

        submeshTriangles[1][27] = 16;
        submeshTriangles[1][28] = 14;
        submeshTriangles[1][29] = 19;

        float totalTrackLength = Vector3.Distance(leftTrackStart, leftTrackEnd);

        uvs.Add(new Vector2(totalTrackLength, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(totalTrackLength, 0));
        uvs.Add(new Vector2(0, 0));

        uvs.Add(new Vector2(totalTrackLength, trackHeight));
        uvs.Add(new Vector2(0, trackHeight));
        uvs.Add(new Vector2(totalTrackLength, trackHeight));
        uvs.Add(new Vector2(0, trackHeight));

        uvs.Add(new Vector2(0, trackHeight + trackWidth));
        uvs.Add(new Vector2(segmentWidth, trackHeight + trackWidth));


        uvs.Add(new Vector2(totalTrackLength, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(totalTrackLength, 0));
        uvs.Add(new Vector2(0, 0));

        uvs.Add(new Vector2(totalTrackLength, trackHeight));
        uvs.Add(new Vector2(0, trackHeight));
        uvs.Add(new Vector2(totalTrackLength, trackHeight));
        uvs.Add(new Vector2(0, trackHeight));

        uvs.Add(new Vector2(0, trackHeight + trackWidth));
        uvs.Add(new Vector2(segmentWidth, trackHeight + trackWidth));
        #endregion

        #region Segments
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 segmentOffset = new Vector3(0, 0, i * segmentWidth + i * spaceBetweenSegments);
            int segmentIndexOffset = trackVerticeCount + segmentVerticeCount * i;

            int currentTriangleArrayIndex = i + 2;
            submeshTriangles[currentTriangleArrayIndex] = new int[54];

            #region Segment Base

            //Front Face
            vertices[0 + segmentIndexOffset] = segmentBaseLowerLeft + segmentOffset;
            vertices[1 + segmentIndexOffset] = segmentBaseLowerRight + segmentOffset;
            vertices[2 + segmentIndexOffset] = segmentMiddleLowerLeft + segmentOffset;
            vertices[3 + segmentIndexOffset] = segmentMiddleLowerRight + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][0] = 0 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][1] = 2 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][2] = 3 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][3] = 0 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][4] = 3 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][5] = 1 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(trackTotalWidth, 0));
            uvs.Add(new Vector2(0, segmentMiddleLowerLeft.y));
            uvs.Add(new Vector2(trackTotalWidth, segmentMiddleLowerLeft.y));

            //Right Face
            vertices[4 + segmentIndexOffset] = segmentBaseLowerRight + segmentOffset;
            vertices[5 + segmentIndexOffset] = segmentBaseUpperRight + segmentOffset;
            vertices[6 + segmentIndexOffset] = segmentMiddleLowerRight + segmentOffset;
            vertices[7 + segmentIndexOffset] = segmentMiddleUpperRight + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][6] = 4 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][7] = 6 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][8] = 7 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][9] = 4 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][10] = 7 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][11] = 5 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(segmentWidth, 0));
            uvs.Add(new Vector2(0, segmentMiddleLowerLeft.y));
            uvs.Add(new Vector2(segmentWidth, segmentMiddleLowerLeft.y));

            //Back Face
            vertices[8 + segmentIndexOffset] = segmentBaseUpperRight + segmentOffset;
            vertices[9 + segmentIndexOffset] = segmentBaseUpperLeft + segmentOffset;
            vertices[10 + segmentIndexOffset] = segmentMiddleUpperRight + segmentOffset;
            vertices[11 + segmentIndexOffset] = segmentMiddleUpperLeft + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][12] = 8 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][13] = 10 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][14] = 11 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][15] = 8 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][16] = 11 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][17] = 9 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(trackTotalWidth, 0));
            uvs.Add(new Vector2(0, segmentMiddleLowerLeft.y));
            uvs.Add(new Vector2(trackTotalWidth, segmentMiddleLowerLeft.y));

            //Left Face
            vertices[12 + segmentIndexOffset] = segmentBaseUpperLeft + segmentOffset;
            vertices[13 + segmentIndexOffset] = segmentBaseLowerLeft + segmentOffset;
            vertices[14 + segmentIndexOffset] = segmentMiddleUpperLeft + segmentOffset;
            vertices[15 + segmentIndexOffset] = segmentMiddleLowerLeft + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][18] = 12 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][19] = 14 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][20] = 15 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][21] = 12 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][22] = 15 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][23] = 13 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(segmentWidth, 0));
            uvs.Add(new Vector2(0, segmentMiddleLowerLeft.y));
            uvs.Add(new Vector2(segmentWidth, segmentMiddleLowerLeft.y));

            #endregion

            #region Segment Top

            float segmentTopAslantHeight = Vector3.Distance(segmentTopLowerLeft, segmentMiddleLowerLeft);
            float segmentTopSideWidth = Vector3.Distance(segmentTopLowerLeft, segmentTopUpperLeft);
            float segmentTopUVOffset = segmentBaseHeightOffset.y + segmentTopAslantHeight;

            //Front Face
            vertices[16 + segmentIndexOffset] = segmentMiddleLowerLeft + segmentOffset;
            vertices[17 + segmentIndexOffset] = segmentMiddleLowerRight + segmentOffset;
            vertices[18 + segmentIndexOffset] = segmentTopLowerLeft + segmentOffset;
            vertices[19 + segmentIndexOffset] = segmentTopLowerRight + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][24] = 16 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][25] = 18 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][26] = 19 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][27] = 16 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][28] = 19 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][29] = 17 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(trackTotalWidth, 0));
            uvs.Add(new Vector2(0, segmentTopAslantHeight));
            uvs.Add(new Vector2(trackTotalWidth, segmentTopAslantHeight));

            //Right Face
            vertices[20 + segmentIndexOffset] = segmentMiddleLowerRight + segmentOffset;
            vertices[21 + segmentIndexOffset] = segmentMiddleUpperRight + segmentOffset;
            vertices[22 + segmentIndexOffset] = segmentTopLowerRight + segmentOffset;
            vertices[23 + segmentIndexOffset] = segmentTopUpperRight + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][30] = 20 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][31] = 22 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][32] = 23 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][33] = 20 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][34] = 23 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][35] = 21 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(segmentWidth, 0));
            uvs.Add(new Vector2((segmentWidth - segmentTopSideWidth) / 2, segmentTopAslantHeight));
            uvs.Add(new Vector2(segmentTopSideWidth + (segmentWidth - segmentTopSideWidth) / 2, segmentTopAslantHeight));

            //Back Face
            vertices[24 + segmentIndexOffset] = segmentMiddleUpperRight + segmentOffset;
            vertices[25 + segmentIndexOffset] = segmentMiddleUpperLeft + segmentOffset;
            vertices[26 + segmentIndexOffset] = segmentTopUpperRight + segmentOffset;
            vertices[27 + segmentIndexOffset] = segmentTopUpperLeft + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][36] = 24 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][37] = 26 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][38] = 27 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][39] = 24 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][40] = 27 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][41] = 25 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(trackTotalWidth, 0));
            uvs.Add(new Vector2(0, segmentTopAslantHeight));
            uvs.Add(new Vector2(trackTotalWidth, segmentTopAslantHeight));

            //Left Face
            vertices[28 + segmentIndexOffset] = segmentMiddleUpperLeft + segmentOffset;
            vertices[29 + segmentIndexOffset] = segmentMiddleLowerLeft + segmentOffset;
            vertices[30 + segmentIndexOffset] = segmentTopUpperLeft + segmentOffset;
            vertices[31 + segmentIndexOffset] = segmentTopLowerLeft + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][42] = 28 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][43] = 30 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][44] = 31 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][45] = 28 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][46] = 31 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][47] = 29 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(segmentWidth, 0));
            uvs.Add(new Vector2((segmentWidth - segmentTopSideWidth) / 2, segmentTopAslantHeight));
            uvs.Add(new Vector2(segmentTopSideWidth + (segmentWidth - segmentTopSideWidth) / 2, segmentTopAslantHeight));

            //Top Face
            vertices[32 + segmentIndexOffset] = segmentTopLowerLeft + segmentOffset;
            vertices[33 + segmentIndexOffset] = segmentTopLowerRight + segmentOffset;
            vertices[34 + segmentIndexOffset] = segmentTopUpperLeft + segmentOffset;
            vertices[35 + segmentIndexOffset] = segmentTopUpperRight + segmentOffset;

            submeshTriangles[currentTriangleArrayIndex][48] = 32 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][49] = 34 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][50] = 35 + segmentIndexOffset;

            submeshTriangles[currentTriangleArrayIndex][51] = 32 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][52] = 35 + segmentIndexOffset;
            submeshTriangles[currentTriangleArrayIndex][53] = 33 + segmentIndexOffset;

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(trackTotalWidth, 0));
            uvs.Add(new Vector2(0, segmentTopSideWidth));
            uvs.Add(new Vector2(trackTotalWidth, segmentTopSideWidth));

            #endregion

            //vertices[4 + segmentIndexOffset] = segmentMiddleLowerLeft + segmentOffset;
            //vertices[5 + segmentIndexOffset] = segmentMiddleUpperLeft + segmentOffset;
            //vertices[6 + segmentIndexOffset] = segmentMiddleUpperRight + segmentOffset;
            //vertices[7 + segmentIndexOffset] = segmentMiddleLowerRight + segmentOffset;

            //vertices[8 + segmentIndexOffset] = segmentTopLowerLeft + segmentOffset;
            //vertices[9 + segmentIndexOffset] = segmentTopUpperLeft + segmentOffset;
            //vertices[10 + segmentIndexOffset] = segmentTopUpperRight + segmentOffset;
            //vertices[11 + segmentIndexOffset] = segmentTopLowerRight + segmentOffset;

            //vertices[12 + segmentIndexOffset] = vertices[8 + segmentIndexOffset];
            //vertices[13 + segmentIndexOffset] = vertices[11 + segmentIndexOffset];

            //vertices[14 + segmentIndexOffset] = vertices[4 + segmentIndexOffset];
            //vertices[15 + segmentIndexOffset] = vertices[0 + segmentIndexOffset];

            //vertices[16 + segmentIndexOffset] = vertices[6 + segmentIndexOffset];
            //vertices[17 + segmentIndexOffset] = vertices[2 + segmentIndexOffset];

            //#region Segment Base Triangles
            //submeshTriangles[currentTriangleArrayIndex][0] = 15 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][1] = 14 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][2] = 3 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][3] = 3 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][4] = 4 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][5] = 7 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][6] = 3 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][7] = 7 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][8] = 2 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][9] = 2 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][10] = 7 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][11] = 6 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][12] = 2 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][13] = 6 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][14] = 1 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][15] = 1 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][16] = 6 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][17] = 5 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][18] = 1 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][19] = 5 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][20] = 4 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][21] = 0 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][22] = 1 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][23] = 4 + segmentIndexOffset;
            //#endregion

            //#region Segment Top Triangles
            //submeshTriangles[currentTriangleArrayIndex][24] = 4 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][25] = 8 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][26] = 7 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][27] = 7 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][28] = 8 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][29] = 11 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][30] = 6 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][31] = 7 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][32] = 11 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][33] = 6 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][34] = 11 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][35] = 10 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][36] = 6 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][37] = 10 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][38] = 5 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][39] = 5 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][40] = 10 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][41] = 9 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][42] = 5 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][43] = 9 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][44] = 8 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][45] = 4 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][46] = 5 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][47] = 8 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][48] = 12 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][49] = 9 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][50] = 13 + segmentIndexOffset;

            //submeshTriangles[currentTriangleArrayIndex][51] = 13 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][52] = 9 + segmentIndexOffset;
            //submeshTriangles[currentTriangleArrayIndex][53] = 10 + segmentIndexOffset;
            //#endregion

            //uvs.Add(new Vector2(0, 0));
            //uvs.Add(new Vector2(trackTotalWidth, 0));
            //uvs.Add(new Vector2(0, 0));
            //uvs.Add(new Vector2(trackTotalWidth, 0));

            //uvs.Add(new Vector2(0, segmentBaseHeightOffset.y));
            //uvs.Add(new Vector2(trackTotalWidth, segmentBaseHeightOffset.y));
            //uvs.Add(new Vector2(0, segmentBaseHeightOffset.y));
            //uvs.Add(new Vector2(trackTotalWidth, segmentBaseHeightOffset.y));

            //float segmentTopAslantHeight = Vector3.Distance(segmentTopLowerLeft, segmentMiddleLowerLeft);
            //float segmentTopUVOffset = segmentBaseHeightOffset.y + segmentTopAslantHeight;
            //uvs.Add(new Vector2(0, segmentTopUVOffset));
            //uvs.Add(new Vector2(trackTotalWidth, segmentTopUVOffset));
            //uvs.Add(new Vector2(0, segmentTopUVOffset));
            //uvs.Add(new Vector2(trackTotalWidth, segmentTopUVOffset));

            //float segmentRoofUVOffset = segmentTopUVOffset + Vector3.Distance(segmentTopLowerLeft, segmentTopUpperLeft);
            //uvs.Add(new Vector2(0, segmentRoofUVOffset));
            //uvs.Add(new Vector2(trackTotalWidth, segmentRoofUVOffset));
        }
        #endregion

        mesh.Clear();
        mesh.vertices = vertices;

        mesh.subMeshCount = segmentCount + 2;
        for (int i = 0; i < segmentCount + 2; i++)
        {
            mesh.SetTriangles(submeshTriangles[i], i);
        }

        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        Material[] materials = new Material[segmentCount + 2];

        materials[0] = Resources.Load<Material>("Materials/metal");
        materials[1] = Resources.Load<Material>("Materials/metal");
        for (int i = 2; i < segmentCount + 2; i++)
        {
            materials[i] = Resources.Load<Material>("Materials/segment");
        }

        meshRenderer.materials = materials;

        meshFilter.sharedMesh = mesh;

    }
}
