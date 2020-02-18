using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class RailTrack : MonoBehaviour
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

    public LinkedList<RailTrackSegment> segments = new LinkedList<RailTrackSegment>();

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    #endregion

    public void CreateMesh()
    {
        //#region Initialization
        //if (!meshFilter)
        //    meshFilter = GetComponent<MeshFilter>();


        //if (!meshRenderer)
        //    meshRenderer = GetComponent<MeshRenderer>();

        //mesh = meshFilter.sharedMesh;

        //if (!mesh)
        //{
        //    mesh = new Mesh();
        //    mesh.name = "Track";
        //}

        //#endregion

        //ConstructedProceduralMesh procMesh = new RailSegmentConstructor
        //{
        //    Base = new Quad()
        //    {
        //        LowerLeft = new Vector3(0, 0, 0),
        //        UpperLeft = new Vector3(0, 0, segmentWidth),
        //        UpperRight = new Vector3(trackTotalWidth, 0, segmentWidth),
        //        LowerRight = new Vector3(trackTotalWidth, 0 ,0)
        //    },
        //    Height = segmentHeight,
        //    HeightCutoutPercentage = segmentCutoutHeightPercentage,
        //    LengthCutoutPercentage = segmentCutoutWidthPercentage
        //}.ConstructMesh();

        //mesh.Clear();
        //mesh.vertices = procMesh.Vertices;
        //mesh.triangles = procMesh.Triangles;

        //mesh.SetUVs(0, procMesh.UVs);

        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        //mesh.RecalculateTangents();

        //meshFilter.sharedMesh = mesh;

    }

    public void Initialize()
    {
        GameObject newTrackSegmentGameObject = new GameObject("Track Segment", typeof(RailTrackSegment));
        newTrackSegmentGameObject.transform.SetParent(transform);
        newTrackSegmentGameObject.transform.localPosition = Vector3.zero;
        RailTrackSegment trackSegment = newTrackSegmentGameObject.GetComponent<RailTrackSegment>();
        trackSegment.Initialize(new TrackConnectionPoint(Vector3.zero, Vector3.forward),
            new TrackConnectionPoint(Vector3.forward * 20 + Vector3.left * 3, Vector3.forward));
        segments.AddLast(trackSegment);
    }
}

public struct TrackConnectionPoint
{
    public Vector3 Point, NormalizedDirection;

    public TrackConnectionPoint(Vector3 point, Vector3 normalizedDirection)
    {
        Point = point;
        NormalizedDirection = normalizedDirection;
    }
}