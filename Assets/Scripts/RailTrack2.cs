using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class RailTrack2 : MonoBehaviour
{
    #region Attributes
    public float trackTotalWidth = 2.8f;
    public float trackWidth = 0.08f;
    public float trackHeight = 0.15f;
    public float segmentWidth = 0.5f;
    public float segmentHeight = 0.22f;
    public int segmentCount = 10;
    public float spaceBetweenSegments = 0.5f;
    public float trackOffsetPercentage = 0.175f;
    public float segmentCutoutWidthPercentage = 0.30f;
    public float segmentCutoutHeightPercentage = 0.65f;

    private Vector3 segmentBaseLowerLeft, segmentBaseUpperLeft, segmentBaseUpperRight, segmentBaseLowerRight,
        segmentMiddleLowerLeft, segmentMiddleUpperLeft, segmentMiddleUpperRight, segmentMiddleLowerRight,
        segmentTopLowerLeft, segmentTopUpperLeft, segmentTopUpperRight, segmentTopLowerRight;

    private Vector3 leftTrackStart, leftTrackEnd, rightTrackStart, rightTrackEnd;


    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh finalMesh;
    #endregion

    public void Awake()
    {   
        CreateMesh();
    }

    public void CreateMesh()
    {
        #region Initialization
        if (!meshFilter)
            meshFilter = GetComponent<MeshFilter>();

        if (!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();

        finalMesh = meshFilter.sharedMesh;

        if (!finalMesh)
        {
            finalMesh = new Mesh();
            finalMesh.name = "Track";
        }
        #endregion

        ConstructedProceduralMesh mesh = new QuadBase
        {
            Width = 4,
            Height = 5,
            Extender = new FlatMeshExtender
            {
                Size = 5,
                ExtensionMode = new StraightExtensionMode(5)
            }
        }.ConstructMesh();

        ;
        Debug.Log(mesh.uvs);

        finalMesh.Clear();
        finalMesh.vertices = mesh.vertices;
        finalMesh.triangles = mesh.triangles;

        finalMesh.SetUVs(0, mesh.uvs);

        finalMesh.RecalculateNormals();
        finalMesh.RecalculateBounds();
        finalMesh.RecalculateTangents();

        meshFilter.sharedMesh = finalMesh;
    }
}
