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

    private Vector3 segmentBaseLowerLeft, segmentBaseUpperLeft, segmentBaseUpperRight, segementBaseLowerRight;

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
            Gizmos.DrawSphere(segementBaseLowerRight + segmentOffset, 0.1f);
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
        segementBaseLowerRight = segmentBaseLowerLeft + new Vector3(trackWidth, 0, 0);
        #endregion
    }
}
