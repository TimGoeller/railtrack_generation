using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RailTrack))]
public class RailTrackEditor : Editor
{
    RailTrack railTrack;

    bool trackSettingsFoldout, segmentSettingsFoldout, trackLineSettingsFoldout;

    private void Awake()
    {
        railTrack = target as RailTrack;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        trackSettingsFoldout = EditorGUILayout.Foldout(trackSettingsFoldout, "Track Settings");
        if(trackSettingsFoldout)
        {
            railTrack.spaceBetweenSegments = EditorGUILayout.FloatField("Space Between Segements", railTrack.spaceBetweenSegments);
        }
        segmentSettingsFoldout = EditorGUILayout.Foldout(segmentSettingsFoldout, "Segment Settings");
        if(segmentSettingsFoldout)
        {
            railTrack.SegmentSettings.SegmentWidth = EditorGUILayout.FloatField("Segment Width", railTrack.SegmentSettings.SegmentWidth);
            railTrack.SegmentSettings.SegmentHeight = EditorGUILayout.FloatField("Segment Height", railTrack.SegmentSettings.SegmentHeight);
            railTrack.SegmentSettings.SegmentLength = EditorGUILayout.FloatField("Segment Length", railTrack.SegmentSettings.SegmentLength);
            railTrack.SegmentSettings.SegmentCutoutHeightPercentage = EditorGUILayout.Slider("Segment Cutout Height", railTrack.SegmentSettings.SegmentCutoutHeightPercentage, 0f, 1f);
            railTrack.SegmentSettings.SegmentCutoutWidthPercentage = EditorGUILayout.Slider("Segment Cutout Width", railTrack.SegmentSettings.SegmentCutoutWidthPercentage, 0f, 1f);
        }
        trackLineSettingsFoldout = EditorGUILayout.Foldout(trackLineSettingsFoldout, "Track Line Settings");
        if (trackLineSettingsFoldout)
        {
            railTrack.TrackSettings.TrackMidPercentage = EditorGUILayout.Slider("Track Offset", railTrack.TrackSettings.TrackMidPercentage, 0.03f, 0.9f);
            railTrack.TrackSettings.TrackOffsetPercentage = EditorGUILayout.Slider("Track Offset", railTrack.TrackSettings.TrackOffsetPercentage, 0.05f, 0.25f);
            railTrack.TrackSettings.TrackWidth = EditorGUILayout.FloatField("Track Width", railTrack.TrackSettings.TrackWidth);
            railTrack.TrackSettings.TrackHeight = EditorGUILayout.FloatField("Track Height", railTrack.TrackSettings.TrackHeight);
        }

        if (EditorGUI.EndChangeCheck())
        {
            //railTrack.();
        }

    }

    void OnSceneGUI()
    {
        if (railTrack.segments.Count == 0)
        {
            railTrack.Initialize();
        }

        if (railTrack.segments.Count != 0)
        {
            RailTrackSegment lastSegment = railTrack.segments.Last.Value;
            Vector3 newConnectionPoint = Handles.FreeMoveHandle(lastSegment.GetEndPosition(), Quaternion.identity, .8f,
                Vector2.zero, Handles.SphereHandleCap);
            newConnectionPoint.y = 0;
            if (lastSegment.GetEndPosition() != newConnectionPoint)
                railTrack.UpdateEnd(newConnectionPoint);
        }


        //DrawHandleOnPlane();
    }

    static Plane XZPlane = new Plane(Vector3.up, Vector3.zero);

    void DrawHandleOnPlane()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        // Undo.RecordObject(creator, "Add segment");
        float distance;
        Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        if (XZPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            //Just double check to ensure the y position is exactly zero
            hitPoint.y = 0;
            Handles.SphereHandleCap(0, hitPoint, Quaternion.identity, 1f, EventType.Repaint);
        }
    }
}
