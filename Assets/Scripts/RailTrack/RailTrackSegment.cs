using System;
using System.CodeDom;
using System.Collections;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Boo.Lang;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine.WSA;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class RailTrackSegment : MonoBehaviour
{
    private RailCurve Curve;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;

    private List<Vector3> samples;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Curve.Start.Point + transform.position, 0.1f);
        Gizmos.DrawSphere(Curve.End.Point + transform.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Curve.BezierHandle + transform.position, 0.2f);

        if (samples != null)
        {
            foreach (Vector3 sample in samples)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(sample + transform.position, 0.1f);
            }
        }
    }

    public Vector3 GetStartPosition()
    {
        return Curve.Start.Point;
    }

    public void SetStartPosition(Vector3 startPosition)
    {
        Curve.Start.Point = startPosition;
    }

    public Vector3 GetEndPosition()
    {
        return Curve.End.Point;
    }

    public void SetEndPosition(Vector3 startPosition)
    {
        Curve.End.Point = startPosition;
    }

    public void RecreateMesh(SegmentSettings settings, TrackSettings trackSettings)
    {
        //Curve.BezierHandle = (Curve.End.Point - Curve.End.Point).magnitude * 0.5f; //TODO Move to Curve Setter
        samples = Curve.CalculateSegmentPoints(0.5f, settings.SegmentWidth, false);
        CreateMesh(samples, settings, trackSettings);
    }

    public void Initialize(TrackConnectionPoint start, TrackConnectionPoint end, SegmentSettings settings, TrackSettings trackSettings, float spacing)
    {
        Curve = new RailCurve(start, end, start.Point + start.NormalizedDirection * (end.Point - start.Point).magnitude * 0.9f);
        samples = Curve.CalculateSegmentPoints(spacing, settings.SegmentWidth, false);
        CreateMesh(samples, settings, trackSettings);
    }


    public void CreateMesh(List<Vector3> samples, SegmentSettings settings, TrackSettings trackSettings)
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
        #endregion

        ConstructedProceduralMesh segmentMesh = new ConstructedProceduralMesh();

        List<Vector3> RailSegmentVerticalNormals = new List<Vector3>();
        List<Vector3> NextSegmentVectors = new List<Vector3>();

        int sampleIndex = 0;
        foreach (Vector3 sample in samples)
        {
            Vector3 vectorToNextSample;
            if (sampleIndex == samples.Count - 1)
            {
                vectorToNextSample = sample - samples[sampleIndex - 1];
            }
            else
            {
                vectorToNextSample = samples[++sampleIndex] - sample;
            }

            vectorToNextSample = vectorToNextSample.normalized;
            NextSegmentVectors.Add(vectorToNextSample);
            Vector3 rotatedVector = (Quaternion.AngleAxis(90, Vector3.up) * vectorToNextSample).normalized;

            RailSegmentVerticalNormals.Add(rotatedVector);

            segmentMesh.AddMesh(
                new RailSegmentConstructor {
                        Base = new Quad()
                        {
                            LowerLeft = sample - (settings.SegmentLength / 2) * rotatedVector,
                            UpperLeft = sample - (settings.SegmentLength / 2) * rotatedVector + vectorToNextSample * settings.SegmentWidth,
                            UpperRight = sample + vectorToNextSample * settings.SegmentWidth + (settings.SegmentLength / 2) * rotatedVector,
                            LowerRight = sample + (settings.SegmentLength / 2) * rotatedVector
                        },
                        Height = settings.SegmentHeight,
                        HeightCutoutPercentage = settings.SegmentCutoutHeightPercentage,
                        LengthCutoutPercentage = settings.SegmentCutoutWidthPercentage
                }.ConstructMesh()
                );
        }

        List<Vector3> leftTrackNodes = new List<Vector3>();
        List<Vector3> rightTrackNodes = new List<Vector3>();

        int index = 0;
        foreach (Vector3 sample in samples)
        {
            leftTrackNodes.Add(
                (sample - (settings.SegmentLength / 2) 
                 * RailSegmentVerticalNormals[index] 
                 * (1 - trackSettings.TrackOffsetPercentage))
                 + new Vector3(0, settings.SegmentHeight, 0)
                 + NextSegmentVectors[index] * (settings.SegmentWidth / 2)
                );

            rightTrackNodes.Add(
                (sample + (settings.SegmentLength / 2)
                 * RailSegmentVerticalNormals[index]
                 * (1 - trackSettings.TrackOffsetPercentage))
                + new Vector3(0, settings.SegmentHeight, 0)
                + NextSegmentVectors[index] * (settings.SegmentWidth / 2)
            );
            index++;
        }

        ConstructedProceduralMesh trackMesh = new ConstructedProceduralMesh();

        trackMesh.AddMesh(new RailConstructor
        {
            RailHeight = trackSettings.TrackHeight,
            RailWidth = trackSettings.TrackWidth,
            RailSegments = leftTrackNodes,
            RailSegmentVerticalNormals = RailSegmentVerticalNormals,
            RailMidPercentage = trackSettings.TrackMidPercentage
        }.ConstructMesh());

        trackMesh.AddMesh(new RailConstructor
        {
            RailHeight = trackSettings.TrackHeight,
            RailWidth = trackSettings.TrackWidth,
            RailSegments = rightTrackNodes,
            RailSegmentVerticalNormals = RailSegmentVerticalNormals,
            RailMidPercentage = trackSettings.TrackMidPercentage
        }.ConstructMesh());

        SubmeshConstructor constructor = new SubmeshConstructor(
            new List<ConstructedProceduralMesh>() {segmentMesh, trackMesh});

        mesh.Clear();
        mesh.vertices = constructor.Vertices;
        mesh.subMeshCount = constructor.SubmeshTriangles.Length;

        int triangleIndex = 0;
        foreach (int[] triangles in constructor.SubmeshTriangles)
        {
            mesh.SetTriangles(triangles, triangleIndex);
            triangleIndex++;
        }

        mesh.SetUVs(0, constructor.UVs);

        Material[] materials = new Material[2];

        materials[0] = Resources.Load<Material>("Materials/segment");
        materials[1] = Resources.Load<Material>("Materials/metal");

        meshRenderer.materials = materials;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshFilter.sharedMesh = mesh;
    }

    public class SegmentSettings
    {
        public float SegmentWidth = 0.5f;
        public float SegmentHeight = 0.22f;
        public float SegmentLength = 2.8f;
        public float SegmentCutoutHeightPercentage = 0.65f;
        public float SegmentCutoutWidthPercentage = 0.30f;
    }

    public class TrackSettings
    {
        public float TrackHeight = 0.15f;
        public float TrackWidth = 0.08f;
        public float TrackOffsetPercentage = 0.175f;
        public float TrackMidPercentage = 0.7f;
    }

    
}
