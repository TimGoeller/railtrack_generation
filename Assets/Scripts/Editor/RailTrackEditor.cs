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

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        railTrack.trackWidth = EditorGUILayout.FloatField("Track Width", railTrack.trackWidth);
        railTrack.segmentWidth = EditorGUILayout.FloatField("Segment Width", railTrack.segmentWidth);
        railTrack.segmentHeight = EditorGUILayout.FloatField("Segment Height", railTrack.segmentHeight);
        railTrack.segmentCount = EditorGUILayout.IntField("Segment Count", railTrack.segmentCount);
        railTrack.spaceBetweenSegments = EditorGUILayout.FloatField("Space Between Segements", railTrack.spaceBetweenSegments);
        railTrack.trackOffsetPercentage = EditorGUILayout.Slider("Track Offset", railTrack.trackOffsetPercentage, 0.05f, 0.25f);

        if(EditorGUI.EndChangeCheck())
        {
            railTrack.CreateMesh();
        }

    }
}
