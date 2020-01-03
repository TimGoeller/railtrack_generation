using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RailTrack))]
public class RailTrackEditor : Editor
{
    #region Attributes
    RailTrack railTrack;
    #endregion

    private void Awake()
    {
        railTrack = target as RailTrack;
    }
}
