using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor 
{
    GameEvent gameEvent;
    List<Type> raisableTypes = new List<Type>(){ typeof(string), typeof(int), typeof(float), typeof(double), typeof(bool) };
    public enum PrimitiveType { None }
    public PrimitiveType primitiveType;

    public override void OnInspectorGUI()
    {
        gameEvent = (GameEvent)target;
        base.OnInspectorGUI();

        bool raisable = isRaiseable();
        Color defaultBackgroundColor = GUI.backgroundColor;
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(gameEvent.isSet);
        if(GUILayout.Button("Set Event"))
        {
            switch (primitiveType)
            {
                case PrimitiveType.None:
                    gameEvent.SetType(null);
                    break;
            }
        }
        EditorGUILayout.EnumPopup(primitiveType);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("Reset Event"))
        {
            gameEvent.UnsetType();
            EditorUtility.SetDirty(gameEvent);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying || !raisable);
        GUI.backgroundColor = Color.green;
        if(GUILayout.Button("Raise Event"))
        {
            if (gameEvent.parameter1Type.type == null)
                gameEvent.Raise(this);
            if (gameEvent.parameter1Type.type != null && gameEvent.parameter2Type.type == null)
                gameEvent.Raise(this, gameEvent.editorParameter1);
            if (gameEvent.parameter1Type.type != null && gameEvent.parameter2Type.type != null && gameEvent.parameter3Type.type == null)
                gameEvent.Raise(this, gameEvent.editorParameter1, gameEvent.editorParameter2);
            if (gameEvent.parameter1Type.type != null && gameEvent.parameter2Type.type != null && gameEvent.parameter3Type.type != null && gameEvent.parameter4Type.type == null)
                gameEvent.Raise(this, gameEvent.editorParameter1, gameEvent.editorParameter2, gameEvent.editorParameter3);
            if (gameEvent.parameter1Type.type != null && gameEvent.parameter2Type.type != null && gameEvent.parameter3Type.type != null && gameEvent.parameter4Type.type != null)
                gameEvent.Raise(this, gameEvent.editorParameter1, gameEvent.editorParameter2, gameEvent.editorParameter3, gameEvent.editorParameter4);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUI.backgroundColor = defaultBackgroundColor;

        SetLabelText(gameEvent);

        if (gameEvent.parameter1Type.type != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parameter 1: " + gameEvent.parameter1Type.type.ToString());
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            gameEvent.editorParameter1 = TypeCheck(gameEvent.editorParameter1, gameEvent.parameter1Type.type);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.LabelField("Parameter 1: None");
        if (gameEvent.parameter2Type.type != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parameter 2: " + gameEvent.parameter2Type.type.ToString());
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            gameEvent.editorParameter2 = TypeCheck(gameEvent.editorParameter2, gameEvent.parameter2Type.type);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.LabelField("Parameter 2: None");
        if (gameEvent.parameter3Type.type != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parameter 3: " + gameEvent.parameter3Type.type.ToString());
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            gameEvent.editorParameter3 = TypeCheck(gameEvent.editorParameter3, gameEvent.parameter3Type.type);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.LabelField("Parameter 3: None");
        if (gameEvent.parameter4Type.type != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parameter 4: " + gameEvent.parameter4Type.type.ToString());
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            gameEvent.editorParameter4 = TypeCheck(gameEvent.editorParameter4, gameEvent.parameter4Type.type);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.LabelField("Parameter 4: None");
    }

    private void SetLabelText(GameEvent @event)
    {
        string text;
        MessageType messageType = MessageType.Info;
        if (!@event.isSet)
        {
            text = "Event not set yet!";
            messageType = MessageType.Warning;
        }
        else
        {
            text = "Event is set up!";
            if (@event.parameter1Type.type == null)
                text += " It has no parameters";
        }
        EditorGUILayout.HelpBox(text, messageType, true);
    }

    private bool isRaiseable()
    {
        if (gameEvent.parameter1Type.type != null && raisableTypes.Contains(gameEvent.parameter1Type.type) && gameEvent.parameter2Type.type == null)
        {
            return true;
        }
        else if (gameEvent.parameter1Type.type != null && raisableTypes.Contains(gameEvent.parameter1Type.type) && gameEvent.parameter2Type.type != null && raisableTypes.Contains(gameEvent.parameter2Type.type) && gameEvent.parameter3Type.type == null)
        {
            return true;
        }
        else if (gameEvent.parameter1Type.type != null && raisableTypes.Contains(gameEvent.parameter1Type.type) && gameEvent.parameter2Type.type != null && raisableTypes.Contains(gameEvent.parameter2Type.type) && gameEvent.parameter3Type.type != null && raisableTypes.Contains(gameEvent.parameter3Type.type) && gameEvent.parameter4Type.type == null)
        {
            return true;
        }
        else if (gameEvent.parameter1Type.type != null && raisableTypes.Contains(gameEvent.parameter1Type.type) && gameEvent.parameter2Type.type != null && raisableTypes.Contains(gameEvent.parameter2Type.type) && gameEvent.parameter3Type.type != null && raisableTypes.Contains(gameEvent.parameter3Type.type) && gameEvent.parameter4Type.type != null && raisableTypes.Contains(gameEvent.parameter4Type.type))
        {
            return true;
        }
        return false;
    }

    private bool MakeBoolField(object target)
    {
        bool value = false;
        if (target != null)
            value = (bool)target;
        value = EditorGUILayout.Toggle(value);
        return value;
    }

    private int MakeIntField(object target)
    {
        int value = 0;
        if (target != null)
             value = (int)target;
        value = EditorGUILayout.IntField(value);
        return value;
    }

    private float MakeFloatField(object target)
    {
        float value = 0;
        if(target != null)
            value = (float)target;
        value = EditorGUILayout.FloatField(value);
        return value;
    }

    private double MakeDoubleField(object target)
    {
        double value = 0;
        if(target != null)
            value = (double)target;
        value = EditorGUILayout.DoubleField(value);
        return value;
    }

    private string MakeStringField(object target)
    {
        string value = "";
        if(target != null)
            value = (string)target;
        value = EditorGUILayout.TextField(value);
        return value;
    }

    private object TypeCheck(object o, Type type)
    {
        if (type == typeof(int))
            return MakeIntField(o);
        else if (type == typeof(bool))
            return MakeBoolField(o);
        else if (type == typeof(string))
            return MakeStringField(o);
        else if (type == typeof(float))
            return MakeFloatField(o);
        else if (type == typeof(double))
            return MakeDoubleField(o);
        return null;
    }
}