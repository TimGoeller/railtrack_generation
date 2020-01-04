using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class RailTrack : MonoBehaviour
{
    #region Attributes
    public float trackWidth = 2;
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


    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    #endregion

    public void OnDrawGizmos()
    {
        for(int i = 0; i < segmentCount; i++)
        {
            Vector3 segmentOffset = new Vector3(0, 0, i * segmentWidth + i * spaceBetweenSegments);
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
        segmentBaseUpperRight = segmentBaseUpperLeft + new Vector3(trackWidth, 0, 0);
        segmentBaseLowerRight = segmentBaseLowerLeft + new Vector3(trackWidth, 0, 0);

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

        #endregion

        #region Segments
        Vector3[] segmentVertices = new Vector3[segmentCount * 14];
        int[][] segmentTriangles = new int[segmentCount][];

        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 segmentOffset = new Vector3(0, 0, i * segmentWidth + i * spaceBetweenSegments);
            int segmentIndexOffset = 14 * i;
            segmentVertices[0 + segmentIndexOffset] = segmentBaseLowerLeft + segmentOffset;
            segmentVertices[1 + segmentIndexOffset] = segmentBaseUpperLeft + segmentOffset;
            segmentVertices[2 + segmentIndexOffset] = segmentBaseUpperRight + segmentOffset;
            segmentVertices[3 + segmentIndexOffset] = segmentBaseLowerRight + segmentOffset;

            segmentVertices[4 + segmentIndexOffset] = segmentMiddleLowerLeft + segmentOffset;
            segmentVertices[5 + segmentIndexOffset] = segmentMiddleUpperLeft + segmentOffset;
            segmentVertices[6 + segmentIndexOffset] = segmentMiddleUpperRight + segmentOffset;
            segmentVertices[7 + segmentIndexOffset] = segmentMiddleLowerRight + segmentOffset;

            segmentVertices[8 + segmentIndexOffset] = segmentTopLowerLeft + segmentOffset;
            segmentVertices[9 + segmentIndexOffset] = segmentTopUpperLeft + segmentOffset;
            segmentVertices[10 + segmentIndexOffset] = segmentTopUpperRight + segmentOffset;
            segmentVertices[11 + segmentIndexOffset] = segmentTopLowerRight + segmentOffset;

            segmentVertices[12 + segmentIndexOffset] = segmentVertices[8 + segmentIndexOffset];
            segmentVertices[13 + segmentIndexOffset] = segmentVertices[11 + segmentIndexOffset];

            segmentTriangles[i] = new int[54];
            #region Segment Base Triangles
            segmentTriangles[i][0] = 0 + segmentIndexOffset;
            segmentTriangles[i][1] = 4 + segmentIndexOffset;
            segmentTriangles[i][2] = 3 + segmentIndexOffset;

            segmentTriangles[i][3] = 3 + segmentIndexOffset;
            segmentTriangles[i][4] = 4 + segmentIndexOffset;
            segmentTriangles[i][5] = 7 + segmentIndexOffset;

            segmentTriangles[i][6] = 3 + segmentIndexOffset;
            segmentTriangles[i][7] = 7 + segmentIndexOffset;
            segmentTriangles[i][8] = 2 + segmentIndexOffset;

            segmentTriangles[i][9] = 2 + segmentIndexOffset;
            segmentTriangles[i][10] = 7 + segmentIndexOffset;
            segmentTriangles[i][11] = 6 + segmentIndexOffset;

            segmentTriangles[i][12] = 2 + segmentIndexOffset;
            segmentTriangles[i][13] = 6 + segmentIndexOffset;
            segmentTriangles[i][14] = 1 + segmentIndexOffset;

            segmentTriangles[i][15] = 1 + segmentIndexOffset;
            segmentTriangles[i][16] = 6 + segmentIndexOffset;
            segmentTriangles[i][17] = 5 + segmentIndexOffset;

            segmentTriangles[i][18] = 1 + segmentIndexOffset;
            segmentTriangles[i][19] = 5 + segmentIndexOffset;
            segmentTriangles[i][20] = 4 + segmentIndexOffset;

            segmentTriangles[i][21] = 0 + segmentIndexOffset;
            segmentTriangles[i][22] = 1 + segmentIndexOffset;
            segmentTriangles[i][23] = 4 + segmentIndexOffset;
            #endregion

            #region Segment Top Triangles
            segmentTriangles[i][24] = 4 + segmentIndexOffset;
            segmentTriangles[i][25] = 8 + segmentIndexOffset;
            segmentTriangles[i][26] = 7 + segmentIndexOffset;

            segmentTriangles[i][27] = 7 + segmentIndexOffset;
            segmentTriangles[i][28] = 8 + segmentIndexOffset;
            segmentTriangles[i][29] = 11 + segmentIndexOffset;

            segmentTriangles[i][30] = 6 + segmentIndexOffset;
            segmentTriangles[i][31] = 7 + segmentIndexOffset;
            segmentTriangles[i][32] = 11 + segmentIndexOffset;

            segmentTriangles[i][33] = 6 + segmentIndexOffset;
            segmentTriangles[i][34] = 11 + segmentIndexOffset;
            segmentTriangles[i][35] = 10 + segmentIndexOffset;

            segmentTriangles[i][36] = 6 + segmentIndexOffset;
            segmentTriangles[i][37] = 10 + segmentIndexOffset;
            segmentTriangles[i][38] = 5 + segmentIndexOffset;

            segmentTriangles[i][39] = 5 + segmentIndexOffset;
            segmentTriangles[i][40] = 10 + segmentIndexOffset;
            segmentTriangles[i][41] = 9 + segmentIndexOffset;

            segmentTriangles[i][42] = 5 + segmentIndexOffset;
            segmentTriangles[i][43] = 9 + segmentIndexOffset;
            segmentTriangles[i][44] = 8 + segmentIndexOffset;

            segmentTriangles[i][45] = 4 + segmentIndexOffset;
            segmentTriangles[i][46] = 5 + segmentIndexOffset;
            segmentTriangles[i][47] = 8 + segmentIndexOffset;

            segmentTriangles[i][48] = 12 + segmentIndexOffset;
            segmentTriangles[i][49] = 9 + segmentIndexOffset;
            segmentTriangles[i][50] = 13 + segmentIndexOffset;

            segmentTriangles[i][51] = 13 + segmentIndexOffset;
            segmentTriangles[i][52] = 9 + segmentIndexOffset;
            segmentTriangles[i][53] = 10 + segmentIndexOffset;
            #endregion

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(segmentWidth, 0));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(segmentWidth, 0));

            uvs.Add(new Vector2(0, segmentBaseHeightOffset.y));
            uvs.Add(new Vector2(segmentWidth, segmentBaseHeightOffset.y));
            uvs.Add(new Vector2(0, segmentBaseHeightOffset.y));
            uvs.Add(new Vector2(segmentWidth, segmentBaseHeightOffset.y));

            float segmentTopAslantHeight = Vector3.Distance(segmentTopLowerLeft, segmentMiddleLowerLeft);
            float segmentTopUVOffset = segmentBaseHeightOffset.y + segmentTopAslantHeight;
            uvs.Add(new Vector2(0, segmentTopUVOffset));
            uvs.Add(new Vector2(segmentWidth, segmentTopUVOffset));
            uvs.Add(new Vector2(0, segmentTopUVOffset));
            uvs.Add(new Vector2(segmentWidth, segmentTopUVOffset));

            float segmentRoofUVOffset = segmentTopUVOffset + Vector3.Distance(segmentTopLowerLeft, segmentTopUpperLeft);
            uvs.Add(new Vector2(0, segmentRoofUVOffset));
            uvs.Add(new Vector2(segmentWidth, segmentRoofUVOffset));
        }

        mesh.Clear();
        mesh.vertices = segmentVertices;

        mesh.subMeshCount = segmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            mesh.SetTriangles(segmentTriangles[i], i);
        }

        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Material[] materials = new Material[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            materials[i] = Resources.Load<Material>("Materials/segment");
        }

        meshRenderer.materials = materials;

        meshFilter.sharedMesh = mesh;

        #endregion
    }
}
