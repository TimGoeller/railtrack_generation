using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTrack : MonoBehaviour
{
    #region Attributes

    public LinkedList<RailTrackSegment> segments = new LinkedList<RailTrackSegment>();

    private RailTrackSegment.SegmentSettings SegmentSettings;

    private RailTrackSegment.TrackSettings TrackSettings;

    private float SpaceBetweenSegments;

    #endregion

    public void ChangeSettings(RailTrackSegment.SegmentSettings segmentSettings,
        RailTrackSegment.TrackSettings trackSettings, float spaceBetweenSegments)
    {
        bool firstSegment = true;
        foreach (RailTrackSegment segment in segments)
        {
            segment.RecreateMesh(segmentSettings, trackSettings, spaceBetweenSegments, firstSegment);
            firstSegment = false;
        }
    }

    public void UpdateEnd(Vector3 connectionPoint)
    {
        segments.Last.Value.SetEndPosition(connectionPoint);
        segments.Last.Value.RecreateMesh(SegmentSettings, TrackSettings, SpaceBetweenSegments, segments.Count == 1);
    }

    public void Initialize(Vector3 start, Vector3 end, RailTrackSegment.SegmentSettings segmentSettings, RailTrackSegment.TrackSettings trackSettings, float spaceBetweenSegments)
    {
        SegmentSettings = segmentSettings;
        TrackSettings = trackSettings;
        SpaceBetweenSegments = spaceBetweenSegments;
        GameObject firstSegmentGO = new GameObject("Track Segment", typeof(RailTrackSegment));
        firstSegmentGO.transform.SetParent(transform);
        firstSegmentGO.transform.localPosition = Vector3.zero;
        RailTrackSegment firstRailTrackSegment = firstSegmentGO.GetComponent<RailTrackSegment>();
        Vector3 normalizedTrackDirection = (end - start).normalized;
        firstRailTrackSegment.Initialize(new TrackConnectionPoint(start, normalizedTrackDirection),
            new TrackConnectionPoint(end, normalizedTrackDirection),
            segmentSettings,
            trackSettings, spaceBetweenSegments, true);
        segments.AddLast(firstRailTrackSegment);
    }

    public void AddSegment(Vector3 end)
    {
        GameObject newSegmentGO = new GameObject("Track Segment", typeof(RailTrackSegment));
        newSegmentGO.transform.SetParent(transform);
        newSegmentGO.transform.localPosition = Vector3.zero;
        RailTrackSegment newRailTrackSegment = newSegmentGO.GetComponent<RailTrackSegment>();
        newRailTrackSegment.Initialize(segments.Last.Value.GetEndConnectionPoint(),
            new TrackConnectionPoint(end, Vector3.zero),
            SegmentSettings,
            TrackSettings, SpaceBetweenSegments, false);
        segments.AddLast(newRailTrackSegment);
    }

    public void DeleteLast()
    {
        RailTrackSegment lastSegment = segments.Last.Value;
        segments.Remove(lastSegment);
        DestroyImmediate(lastSegment.gameObject);
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