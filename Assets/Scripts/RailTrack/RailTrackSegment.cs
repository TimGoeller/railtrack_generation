using System;
using System.Linq;
using Boo.Lang;
using UnityEngine;

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

    public Vector3 GetEndPosition()
    {
        return Curve.End.Point;
    }

    public void SetEndPosition(Vector3 startPosition)
    {
        Curve.End.Point = startPosition;
    }

    public TrackConnectionPoint GetEndConnectionPoint()
    {
        return Curve.End;
    }

    public void RecreateMesh(SegmentSettings settings, TrackSettings trackSettings, float spacing, bool firstSegment)
    {
        Curve.BezierHandle = Curve.Start.Point +
                             Curve.Start.NormalizedDirection * (Curve.End.Point - Curve.Start.Point).magnitude ;
        samples = Curve.CalculateSegmentPoints(spacing, settings.SegmentWidth, !firstSegment);
        CreateMesh(samples, settings, trackSettings, !firstSegment);
    }

    public void Initialize(TrackConnectionPoint start, TrackConnectionPoint end, SegmentSettings settings, TrackSettings trackSettings, float spacing, bool firstSegment)
    {
        Curve = new RailCurve(start, end, start.Point + start.NormalizedDirection * (end.Point - start.Point).magnitude * 0.9f);
        samples = Curve.CalculateSegmentPoints(spacing, settings.SegmentWidth, !firstSegment);
        CreateMesh(samples, settings, trackSettings, !firstSegment);
    }


    public void CreateMesh(List<Vector3> samples, SegmentSettings settings, TrackSettings trackSettings, bool connectPrevious)
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

        Vector3 vectorToNextSample = Vector3.zero;

        int sampleIndex = 0;
        foreach (Vector3 sample in samples)
        {

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

        Curve.End.NormalizedDirection = vectorToNextSample;

        List<Vector3> leftTrackNodes = new List<Vector3>();
        List<Vector3> rightTrackNodes = new List<Vector3>();

        int index = 0;

        if (connectPrevious)
        {
            Vector3 startOnLastTrack = (Curve.Start.Point) - (settings.SegmentWidth) * Curve.Start.NormalizedDirection;
            Vector3 vectorToSegmentStart = samples.First() - startOnLastTrack;
            Vector3 rotatedVector = (Quaternion.AngleAxis(90, Vector3.up) * Curve.Start.NormalizedDirection);

            RailSegmentVerticalNormals.Insert(0, rotatedVector);

            leftTrackNodes.Add(
                (startOnLastTrack - (settings.SegmentLength / 2)
                 * rotatedVector
                 * (1 - trackSettings.TrackOffsetPercentage))
                + new Vector3(0, settings.SegmentHeight, 0)
                + vectorToSegmentStart * (settings.SegmentWidth / 2)
            );

            rightTrackNodes.Add(
                (startOnLastTrack + (settings.SegmentLength / 2)
                 * rotatedVector
                 * (1 - trackSettings.TrackOffsetPercentage))
                + new Vector3(0, settings.SegmentHeight, 0)
                + vectorToSegmentStart * (settings.SegmentWidth / 2)
            );
        }

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
