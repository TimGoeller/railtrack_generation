using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(RailTrack))]
public class RailTrackEditor : Editor
{
    RailTrack railTrack;

    bool trackSettingsFoldout, segmentSettingsFoldout, trackLineSettingsFoldout;

    private bool leftMouseHeldDown;

    private RailTrackEditorMode mode = RailTrackEditorMode.EditLastSegment;

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
        //Don't lose focus when clicking into the scene
        Selection.activeGameObject = railTrack.transform.gameObject;

        DrawUI();
        HandleInteractions();
    }

    private void DrawUI()
    {
        Handles.BeginGUI();

        Color standardBackgroundColor = GUI.backgroundColor;

        if (mode == RailTrackEditorMode.EditLastSegment)
        {
            GUI.backgroundColor = Color.red;
        }

        if (GUI.Button(new Rect(10, 10, 100, 50), "Edit Segment"))
        {
            if(mode != RailTrackEditorMode.EditLastSegment)
                SwitchToEditMode();
        }

        if (mode == RailTrackEditorMode.AddNewSegment)
        {
            GUI.backgroundColor = Color.red;
        }
        else
        {
            GUI.backgroundColor = standardBackgroundColor;
        }

        if (GUI.Button(new Rect(130, 10, 100, 50), "Add Segment"))
        {
            if(mode != RailTrackEditorMode.AddNewSegment)
                SwitchToAddMode();
        }

        Handles.EndGUI();
    }

    private void SwitchToEditMode()
    {
        mode = RailTrackEditorMode.EditLastSegment;
    }

    private void SwitchToAddMode()
    {
        mode = RailTrackEditorMode.AddNewSegment;
    }

    private void HandleInteractions()
    {
        if (mode == RailTrackEditorMode.EditLastSegment)
        {
            CheckLeftMouseButtonStatus();
        }

        Event guiEvent = Event.current;

        if (railTrack.segments.Count != 0)
        {
            RailTrackSegment lastSegment = railTrack.segments.Last.Value;

            if (mode == RailTrackEditorMode.EditLastSegment)
            {
                Handles.FreeMoveHandle(lastSegment.GetEndPosition(), Quaternion.identity, .8f,
                    Vector2.zero, Handles.SphereHandleCap);

                if (leftMouseHeldDown)
                {
                    RaycastUtil.XZRaycast raycast = RaycastUtil.VectorToXZPlane(guiEvent.mousePosition);

                    if (raycast.Hit)
                    {
                        if (lastSegment.GetEndPosition() != raycast.HitPoint)
                            railTrack.UpdateEnd(raycast.HitPoint);
                    }
                }
            }
            else if (mode == RailTrackEditorMode.AddNewSegment)
            {
                RaycastUtil.XZRaycast raycast = RaycastUtil.VectorToXZPlane(guiEvent.mousePosition);

                if (Event.current.type == EventType.MouseDown)
                {
                    if (guiEvent.button == (int)MouseButton.LeftMouse)
                    {
                        if (raycast.Hit)
                        {
                            railTrack.AddSegment(raycast.HitPoint);
                        }
                    }
                }


            }

        }
    }

    private void CheckLeftMouseButtonStatus()
    {
        Event guiEvent = Event.current;

        if (Event.current.type == EventType.MouseDown)
        {
            if (guiEvent.button == (int)MouseButton.LeftMouse)
            {
                leftMouseHeldDown = true;
            }
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            if (guiEvent.button == (int)MouseButton.LeftMouse)
            {
                leftMouseHeldDown = false;
            }
        }
    }

    enum RailTrackEditorMode
    {
        EditLastSegment,
        AddNewSegment
    }

}
