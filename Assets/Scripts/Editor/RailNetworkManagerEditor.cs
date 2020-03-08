﻿using System;
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

    private RailEditorMode mode = RailEditorMode.InitialTrackStartPointPlacement;

    private void Awake()
    {
        railNetworkManager = target as RailNetworkManager;
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
        railTrack.Initialize(start, end);
    }

    enum RailEditorMode
    {
        InitialTrackStartPointPlacement,
        InitialTrackEndPointPlacement
    }
}