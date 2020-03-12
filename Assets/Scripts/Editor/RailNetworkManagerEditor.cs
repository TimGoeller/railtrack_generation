using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.XR;

[CustomEditor(typeof(RailNetworkManager))]
public class RailNetworkManagerEditor : Editor
{
    RailNetworkManager railNetworkManager;

    private RailTrack currentNewTrack;

    private Vector3 InitialPlacementPoint;

    bool trackSettingsFoldout, segmentSettingsFoldout, trackLineSettingsFoldout;

    private RailEditorMode mode = RailEditorMode.InitialTrackStartPointPlacement;

    private void Awake()
    {
        railNetworkManager = target as RailNetworkManager;
    }


    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        trackSettingsFoldout = EditorGUILayout.Foldout(trackSettingsFoldout, "Track Settings");
        if (trackSettingsFoldout)
        {
            railNetworkManager.spaceBetweenSegments = EditorGUILayout.FloatField("Space Between Segements", railNetworkManager.spaceBetweenSegments);
        }
        segmentSettingsFoldout = EditorGUILayout.Foldout(segmentSettingsFoldout, "Segment Settings");
        if (segmentSettingsFoldout)
        {
            railNetworkManager.SegmentSettings.SegmentWidth = EditorGUILayout.FloatField("Segment Width", railNetworkManager.SegmentSettings.SegmentWidth);
            railNetworkManager.SegmentSettings.SegmentHeight = EditorGUILayout.FloatField("Segment Height", railNetworkManager.SegmentSettings.SegmentHeight);
            railNetworkManager.SegmentSettings.SegmentLength = EditorGUILayout.FloatField("Segment Length", railNetworkManager.SegmentSettings.SegmentLength);
            railNetworkManager.SegmentSettings.SegmentCutoutHeightPercentage = EditorGUILayout.Slider("Segment Cutout Height", railNetworkManager.SegmentSettings.SegmentCutoutHeightPercentage, 0f, 1f);
            railNetworkManager.SegmentSettings.SegmentCutoutWidthPercentage = EditorGUILayout.Slider("Segment Cutout Width", railNetworkManager.SegmentSettings.SegmentCutoutWidthPercentage, 0f, 1f);
        }
        trackLineSettingsFoldout = EditorGUILayout.Foldout(trackLineSettingsFoldout, "Track Line Settings");
        if (trackLineSettingsFoldout)
        {
            railNetworkManager.TrackSettings.TrackMidPercentage = EditorGUILayout.Slider("Track Offset", railNetworkManager.TrackSettings.TrackMidPercentage, 0.03f, 0.9f);
            railNetworkManager.TrackSettings.TrackOffsetPercentage = EditorGUILayout.Slider("Track Offset", railNetworkManager.TrackSettings.TrackOffsetPercentage, 0.05f, 0.25f);
            railNetworkManager.TrackSettings.TrackWidth = EditorGUILayout.FloatField("Track Width", railNetworkManager.TrackSettings.TrackWidth);
            railNetworkManager.TrackSettings.TrackHeight = EditorGUILayout.FloatField("Track Height", railNetworkManager.TrackSettings.TrackHeight);
        }

        if (EditorGUI.EndChangeCheck())
        {
            foreach (RailTrack track in railNetworkManager.RailTracks)
            {
                track.ChangeSettings(railNetworkManager.SegmentSettings, railNetworkManager.TrackSettings,
                    railNetworkManager.spaceBetweenSegments);
            }
        }

    }

    void OnSceneGUI()
    {
        //Don't lose focus when clicking into the scene
        Selection.activeGameObject = railNetworkManager.transform.gameObject;

        if (mode == RailEditorMode.InitialTrackEndPointPlacement)
        {
            Handles.SphereHandleCap(0, InitialPlacementPoint, Quaternion.identity, 0.5f, EventType.Repaint);
        }

        DrawSceneUI();
        HandleInteractions();
    }

    void DrawSceneUI()
    {
        Handles.BeginGUI();
        GUI.Label(new Rect(10, 0, 300, 50), "Click on two locations to place a new track");
        Handles.EndGUI();
    }

    void HandleInteractions()
    {
        Event guiEvent = Event.current;

        if (Event.current.type == EventType.MouseDown)
        {
            if (guiEvent.button == (int) MouseButton.LeftMouse)
            {
                RaycastUtil.XZRaycast raycast = RaycastUtil.VectorToXZPlane(guiEvent.mousePosition);

                if (raycast.Hit)
                {
                    if (mode == RailEditorMode.InitialTrackStartPointPlacement)
                    {
                        SwitchToEndPointPlacementMode(raycast.HitPoint);
                    }
                    else if (mode == RailEditorMode.InitialTrackEndPointPlacement)
                    {
                        PlaceTrack(InitialPlacementPoint, raycast.HitPoint);
                        SwitchToStartPointPlacementMode();
                    }
                }
            }
            else if (guiEvent.button == (int) MouseButton.RightMouse)
            {
                SwitchToStartPointPlacementMode();
            }
        }
    }

    public void SwitchToEndPointPlacementMode(Vector3 startPoint)
    {
        InitialPlacementPoint = startPoint;
        mode = RailEditorMode.InitialTrackEndPointPlacement;
    }

    public void SwitchToStartPointPlacementMode()
    {
        mode = RailEditorMode.InitialTrackStartPointPlacement;
    }

    public void PlaceTrack(Vector3 start, Vector3 end)
    {
        GameObject newTrackGameObject = new GameObject("Rail Track", typeof(RailTrack));
        newTrackGameObject.transform.parent = railNetworkManager.transform;
        newTrackGameObject.transform.position = Vector3.zero;
        RailTrack railTrack = newTrackGameObject.GetComponent<RailTrack>();
        railNetworkManager.RailTracks.Add(railTrack);
        railTrack.Initialize(start, end, railNetworkManager.SegmentSettings, railNetworkManager.TrackSettings, railNetworkManager.spaceBetweenSegments);
    }

    enum RailEditorMode
    {
        InitialTrackStartPointPlacement,
        InitialTrackEndPointPlacement
    }
}