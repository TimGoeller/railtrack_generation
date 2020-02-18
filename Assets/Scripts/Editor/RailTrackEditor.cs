using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RailTrack))]
public class RailTrackEditor : Editor
{
    #region Attributes
    RailTrack railTrack;

    bool trackSettingsFoldout, segmentSettingsFoldout, trackLineSettingsFoldout;
    #endregion

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
            railTrack.trackTotalWidth = EditorGUILayout.FloatField("Total Track Width", railTrack.trackTotalWidth);
            railTrack.segmentCount = EditorGUILayout.IntField("Segment Count", railTrack.segmentCount);
            railTrack.spaceBetweenSegments = EditorGUILayout.FloatField("Space Between Segements", railTrack.spaceBetweenSegments);
        }
        segmentSettingsFoldout = EditorGUILayout.Foldout(segmentSettingsFoldout, "Segment Settings");
        if(segmentSettingsFoldout)
        {
            railTrack.segmentWidth = EditorGUILayout.FloatField("Segment Width", railTrack.segmentWidth);
            railTrack.segmentHeight = EditorGUILayout.FloatField("Segment Height", railTrack.segmentHeight);
            railTrack.segmentCutoutHeightPercentage = EditorGUILayout.Slider("Segment Cutout Height", railTrack.segmentCutoutHeightPercentage, 0f, 1f);
            railTrack.segmentCutoutWidthPercentage = EditorGUILayout.Slider("Segment Cutout Width", railTrack.segmentCutoutWidthPercentage, 0f, 1f);
        }
        trackLineSettingsFoldout = EditorGUILayout.Foldout(trackLineSettingsFoldout, "Track Line Settings");
        if (trackLineSettingsFoldout)
        {
            railTrack.trackOffsetPercentage = EditorGUILayout.Slider("Track Offset", railTrack.trackOffsetPercentage, 0.05f, 0.25f);
            railTrack.trackWidth = EditorGUILayout.FloatField("Track Width", railTrack.trackWidth);
            railTrack.trackHeight = EditorGUILayout.FloatField("Track Height", railTrack.trackHeight);
        }

        if (EditorGUI.EndChangeCheck())
        {
            railTrack.CreateMesh();
        }

    }

    void OnSceneGUI()
    {
        if (railTrack.segments.Count == 0)
        {
            railTrack.Initialize();
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
