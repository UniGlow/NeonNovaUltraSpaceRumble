using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Reflection;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(GameEventListener))]
public class GameEventListenerEditor : Editor
{
    GameEventListener script;
    ReorderableList linkedMethodsList;

    private void OnEnable()
    {
        script = (GameEventListener)target;
        if (script.linkedMethods == null)
        {
            script.linkedMethods = new List<LinkedMethod>();
        }
        linkedMethodsList = new ReorderableList(serializedObject, serializedObject.FindProperty("linkedMethods"), false, true, true, true);
        linkedMethodsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                //SerializedProperty element = linkedMethodsList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                script.linkedMethods[index].component = EditorGUI.ObjectField(
                    new Rect(rect.x + 10, rect.y, (rect.width / 2) - 10, EditorGUIUtility.singleLineHeight),
                    script.linkedMethods[index].component, typeof(Component), true) as Component;
                string log = "Starting search for behaviour!";
                // Logic for what Methods to Display
                Component component = null;
                List<MethodInfo> methods = new List<MethodInfo>();
                List<int> methodIDs = new List<int>();
                string[] popupOptions;
                MethodInfo[] tempMethods = null;
                if (script.linkedMethods[index].component != null)
                {
                    log += "\nBehaviour found! Searching for suitable methods!";
                    component = script.linkedMethods[index].component;
                    tempMethods = component.GetType().GetMethods();
                    log += "\nStep 1: Searching for all methods";
                    foreach(MethodInfo info in tempMethods)
                    {
                        log += "\n\t" + info.Name + "(";
                        for (int i = 0; i < info.GetParameters().Length; i++)
                        {
                            log += info.GetParameters()[i].ParameterType + " " + info.GetParameters()[i].Name;
                        }
                        log += ")";
                    }
                    if (script.gameEvent.parameter1Type.type != null && script.gameEvent.parameter2Type.type == null)
                    {
                        for(int i = 0; i < tempMethods.Length; i++)
                        {
                            MethodInfo info = tempMethods[i];
                            if (info.GetParameters().Length == 1 && info.GetParameters()[0].ParameterType == script.gameEvent.parameter1Type.type)
                            {
                                methods.Add(info);
                                methodIDs.Add(i);
                            }
                        }
                    }else if(script.gameEvent.parameter1Type.type != null && script.gameEvent.parameter2Type.type != null && script.gameEvent.parameter3Type.type == null)
                    {
                        for(int i = 0; i < tempMethods.Length; i++)
                        {
                            MethodInfo info = tempMethods[i];
                            if(info.GetParameters().Length == 2 && info.GetParameters()[0].ParameterType == script.gameEvent.parameter1Type.type && info.GetParameters()[1].ParameterType == script.gameEvent.parameter2Type.type)
                            {
                                methods.Add(info);
                                methodIDs.Add(i);
                            }
                        }
                    }else if(script.gameEvent.parameter1Type.type != null && script.gameEvent.parameter2Type.type != null && script.gameEvent.parameter3Type.type != null && script.gameEvent.parameter4Type.type == null)
                    {
                        for(int i = 0; i < tempMethods.Length; i++)
                        {
                            MethodInfo info = tempMethods[i];
                            if (info.GetParameters().Length == 3 && info.GetParameters()[0].ParameterType == script.gameEvent.parameter1Type.type && info.GetParameters()[1].ParameterType == script.gameEvent.parameter2Type.type && info.GetParameters()[2].ParameterType == script.gameEvent.parameter3Type.type)
                            {
                                methods.Add(info);
                                methodIDs.Add(i);
                            }
                        }
                    }
                    else if (script.gameEvent.parameter1Type.type != null && script.gameEvent.parameter2Type.type != null && script.gameEvent.parameter3Type.type != null && script.gameEvent.parameter4Type.type != null)
                    {
                        for(int i = 0; i < tempMethods.Length; i++)
                        {
                            MethodInfo info = tempMethods[i];
                            if (info.GetParameters().Length == 4 && info.GetParameters()[0].ParameterType == script.gameEvent.parameter1Type.type && info.GetParameters()[1].ParameterType == script.gameEvent.parameter2Type.type && info.GetParameters()[2].ParameterType == script.gameEvent.parameter3Type.type && info.GetParameters()[3].ParameterType == script.gameEvent.parameter4Type.type)
                            {
                                methods.Add(info);
                                methodIDs.Add(i);
                            }
                        }
                    }
                    log += "\nStep 2: Filtering methods for parameter types";
                    log += "\n\tFiltered Methods:";
                }
                if (methods.Count != 0)
                {
                    methods.ForEach((MethodInfo m) =>
                    {
                        log += "\n\t\t" + m.Name;
                    });
                    popupOptions = new string[methods.Count+1];
                    popupOptions[0] = "None";
                    for (int i = 1; i <= methods.Count; i++)
                    {
                        popupOptions[i] = methods[i-1].Name;
                    }
                    if (script.linkedMethods[index].chosenID == -1)
                        script.linkedMethods[index].chosenID = 0;
                    Undo.RecordObject(script, "Change Selection of Response Method");
                    script.linkedMethods[index].chosenID = EditorGUI.Popup(
                        new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                        script.linkedMethods[index].chosenID, popupOptions);
                    EditorUtility.SetDirty(script);
                    
                    script.linkedMethods[index].gameObject = component.gameObject;
                    script.linkedMethods[index].methodID = script.linkedMethods[index].chosenID > 0 ? methodIDs[script.linkedMethods[index].chosenID - 1] : 0;
                    script.linkedMethods[index].methodName = popupOptions[script.linkedMethods[index].chosenID];
                    /*for (int i = 0; i < tempMethods.Length; i++)
                    {
                        if (tempMethods[i].Name == popupOptions[script.linkedMethods[index].chosenID])
                        {
                            script.linkedMethods[index].gameObject = component.gameObject;
                            script.linkedMethods[index].methodID = i;
                            script.linkedMethods[index].methodName = popupOptions[script.linkedMethods[index].chosenID];
                        }
                    }*/
                }
                else
                {
                    log += "\n\tNo suitable methods found!!!";
                    //EditorStyles.popup.
                    EditorGUI.Popup(
                        new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                        0, new string[] { "No suiting methods found!" });
                    script.linkedMethods[index].chosenID = -1;
                    script.linkedMethods[index].methodName = null;
                    script.linkedMethods[index].methodID = -1;
                }
                //Debug.Log(log);
            };;
        linkedMethodsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Response ()");
        };
        linkedMethodsList.onAddCallback = (ReorderableList l) =>
        {
            int index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            script.linkedMethods.Add(new LinkedMethod());
            SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("component").objectReferenceValue = null;
            element.FindPropertyRelative("methodName").stringValue = null;
        };
        linkedMethodsList.onRemoveCallback = (ReorderableList l) =>
        {
            l.serializedProperty.DeleteArrayElementAtIndex(l.index);
        };
    }

    public override void OnInspectorGUI()
    {
        script = (GameEventListener)target;
        EditorGUILayout.Space();
        script.gameEvent = EditorGUILayout.ObjectField("Event", script.gameEvent, typeof(GameEvent), true) as GameEvent;
        if(script.gameEvent != null)
        {
            SetLabelText(script.gameEvent);
            EditorGUILayout.Space();
            GameEvent gameEvent = script.gameEvent;
            if (gameEvent.isSet)
            {
                if (gameEvent.parameter1Type.type != null)
                {
                    script.ignoreEventParameters = EditorGUILayout.Toggle("Ignore Event parameters", script.ignoreEventParameters);
                    Undo.RecordObject(script, "Changed if ignoring Event parameters");
                }

                if (gameEvent.parameter1Type.type == null || script.ignoreEventParameters)
                {
                    serializedObject.Update();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("response"));
                    serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    serializedObject.Update();
                    linkedMethodsList.DoLayoutList();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
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
            if (@event.parameter1Type.type == null)
                text = "Parameter type: None";
            else
            {
                text = "Parameter type";
                if (@event.parameter2Type.type != null)
                    text += "s";
                text += ": " + TypeName(@event.parameter1Type.type);
                if (@event.parameter2Type.type != null)
                    text += " | " + TypeName(@event.parameter2Type.type);
                if (@event.parameter3Type.type != null)
                    text += " | " + TypeName(@event.parameter3Type.type);
                if (@event.parameter4Type.type != null)
                    text += " | " + TypeName(@event.parameter4Type.type);
            }
        }
        EditorGUILayout.HelpBox(text, messageType, true);
    }

    private string TypeName(Type t)
    {
        string fullName = t.Name;
        string shortenedName;
        for(int i=fullName.Length-1; i > 0; i--)
        {
            if (fullName[i] == '.')
            {
                shortenedName = fullName.Substring(i + 1);
                return shortenedName;
            }
        }
        return fullName;
    }
}