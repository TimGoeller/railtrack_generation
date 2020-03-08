using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(RailNetworkManager))]
public class RailNetworkManagerEditor : Editor
{
    RailNetworkManager railNetworkManager;

    private RailTrack currentNewTrack;

    private Vector3 InitialPlacementPoint;

    private RailEditorMode mode;

    private void Awake()
    {
        railNetworkManager = target as RailNetworkManager;
    }


    void OnSceneGUI()
    {
        if (mode == RailEditorMode.InitialTrackEndPointPlacement)
        {
            Handles.SphereHandleCap(0, InitialPlacementPoint, Quaternion.identity, 0.5f, EventType.Repaint);
        }

        DrawUI();
        HandleNewPlacementInitiations();

    }

    void DrawUI()
    {
        Handles.BeginGUI();

        Color standardBackgroundColor = GUI.backgroundColor;

        if (mode == RailEditorMode.InitialTrackStartPointPlacement ||
            mode == RailEditorMode.InitialTrackEndPointPlacement)
        {
            GUI.backgroundColor = Color.red;
            if (GUI.Button(new Rect(10, 10, 100, 50), "End Placement"))
            {
                mode = RailEditorMode.None;
            }

            GUI.backgroundColor = standardBackgroundColor; }
        else
        {
            if (GUI.Button(new Rect(10, 10, 100, 50), "Place Track"))
                mode = RailEditorMode.InitialTrackStartPointPlacement;
        }

        Handles.EndGUI();
    }

    void HandleNewPlacementInitiations()
    {
        if(mode == RailEditorMode.InitialTrackStartPointPlacement || mode == RailEditorMode.InitialTrackEndPointPlacement)
            Selection.activeGameObject = railNetworkManager.transform.gameObject;

        Event guiEvent = Event.current;

        if (Event.current.type == EventType.MouseDown)
        {
            if (guiEvent.button == (int)MouseButton.LeftMouse)
            {
                if (mode == RailEditorMode.InitialTrackStartPointPlacement)
                {
                    var plane = new Plane(Vector3.up, Vector3.zero);
                    var ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);

                    float distance;

                    if (plane.Raycast(ray, out distance))
                    {
                        InitialPlacementPoint = ray.GetPoint(distance);
                        mode = RailEditorMode.InitialTrackEndPointPlacement;
                    }
                }

                
            }

            if (guiEvent.button == (int) MouseButton.RightMouse)
            {
                if (mode == RailEditorMode.InitialTrackEndPointPlacement && guiEvent.shift)
                {
                    mode = RailEditorMode.InitialTrackStartPointPlacement;
                }
            }
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            if (mode == RailEditorMode.InitialTrackEndPointPlacement)
            {
                var plane = new Plane(Vector3.up, Vector3.zero);
                var ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);

                float distance;

                if (plane.Raycast(ray, out distance))
                {
                    Vector3 secondPlacementPoint = ray.GetPoint(distance);
                    PlaceNewTrack(InitialPlacementPoint, secondPlacementPoint);
                    mode = RailEditorMode.InitialTrackStartPointPlacement;
                }
            }
        }
        else if (Event.current.type == EventType.MouseMove)
        {
             
        }


    }

    public void PlaceNewTrack(Vector3 start, Vector3 end)
    {
        Debug.Log("Place Track " + start + end);
        GameObject newTrackGameObject = new GameObject("Rail Track", typeof(RailTrack));
        newTrackGameObject.transform.parent = railNetworkManager.transform;
        newTrackGameObject.transform.position = Vector3.zero;
        RailTrack railTrack = newTrackGameObject.GetComponent<RailTrack>();
        railTrack.Initialize(start, end);
    }

    enum RailEditorMode
    {
        None,
        InitialTrackStartPointPlacement,
        InitialTrackEndPointPlacement
    }
}