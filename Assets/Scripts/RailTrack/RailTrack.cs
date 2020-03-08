using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTrack : MonoBehaviour
{
    #region Attributes
    public float spaceBetweenSegments = 0.5f;

    public RailTrackSegment.TrackSettings TrackSettings = new RailTrackSegment.TrackSettings();
    public RailTrackSegment.SegmentSettings SegmentSettings = new RailTrackSegment.SegmentSettings();

    public LinkedList<RailTrackSegment> segments = new LinkedList<RailTrackSegment>();
    #endregion

    public void UpdateEnd(Vector3 connectionPoint)
    {
        segments.Last.Value.SetEndPosition(connectionPoint);
        segments.Last.Value.RecreateMesh(SegmentSettings, TrackSettings);
    }

    public void Initialize(Vector3 start, Vector3 end)
    {
        GameObject firstSegmentGO = new GameObject("Track Segment", typeof(RailTrackSegment));
        firstSegmentGO.transform.SetParent(transform);
        firstSegmentGO.transform.localPosition = Vector3.zero;
        RailTrackSegment firstRailTrackSegment = firstSegmentGO.GetComponent<RailTrackSegment>();
        Vector3 normalizedTrackDirection = (end - start).normalized;
        firstRailTrackSegment.Initialize(new TrackConnectionPoint(start, normalizedTrackDirection),
            new TrackConnectionPoint(end, normalizedTrackDirection),
            SegmentSettings,
            TrackSettings, spaceBetweenSegments);
        segments.AddLast(firstRailTrackSegment);
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