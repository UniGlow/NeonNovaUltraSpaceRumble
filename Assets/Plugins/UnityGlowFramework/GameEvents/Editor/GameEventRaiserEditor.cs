using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEventRaiser))]
public class GameEventRaiserEditor : Editor
{
    GameEventRaiser eventRaiser;
    public override void OnInspectorGUI()
    {
        eventRaiser = (GameEventRaiser)target;
        eventRaiser.gameEvent = EditorGUILayout.ObjectField("Game Event", eventRaiser.gameEvent, typeof(GameEvent), false) as GameEvent;
        EditorGUILayout.HelpBox("Only parameterless GameEvents are supported!", MessageType.Warning);
    }
}