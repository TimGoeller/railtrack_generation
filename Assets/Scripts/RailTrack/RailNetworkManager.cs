using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailNetworkManager : MonoBehaviour
{
    public List<RailTrack> RailTracks;

    public RailTrackSegment.TrackSettings TrackSettings = new RailTrackSegment.TrackSettings();
    public RailTrackSegment.SegmentSettings SegmentSettings = new RailTrackSegment.SegmentSettings();

    public float spaceBetweenSegments = 0.5f;
}
