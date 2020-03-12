using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(RailTrack))]
public class RailTrackEditor : Editor
{
    RailTrack railTrack;

    private bool leftMouseHeldDown;

    private RailTrackEditorMode mode = RailTrackEditorMode.EditLastSegment;

    private void Awake()
    {
        railTrack = target as RailTrack;
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

        GUI.backgroundColor = standardBackgroundColor;

        if (GUI.Button(new Rect(250, 10, 100, 50), "Delete Last"))
        {
            railTrack.DeleteLast();
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
