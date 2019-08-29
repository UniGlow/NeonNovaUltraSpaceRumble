using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[CustomPropertyDrawer(typeof(GameEvent))]
public class GameEventPropertyDrawer : PropertyDrawer
{
    bool tooMuchParameters = false;
    bool noMethodsFound = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Color defaultBackgroundColor = GUI.backgroundColor;
        
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // TODO: Find out the State that gets drawn
        int heightState = 0;        // Height State to Draw the Property!
                                    // 0 = Property is one Line high
                                    // 1 = Property is two Lines high
                                    // 2 = Property is four Lines high
                                    // 3 = Property is five Lines high
        heightState = GetHeightState(property);

        //Rect for Game Event Reference
        //if Reference not set: The height will be only one line, PropertyField will use the full height, and the full width
        //else: Window will be higher and potentially a Button next to the PropertyField
        Rect valueRect;

        //Rect for Set-Button
        //when shown is always one line high and 50 Pixel wide
        Rect buttonRect;

        //Rect for TypeLabel
        //when shown is always one line high and full width
        Rect labelField;

        //Rect for HelperBox
        //This thing uses 3 lines height but has half a line space top and bottom
        //It also stretches out the full width of the inspector window
        Rect helperBox;

        switch (heightState)
        {
            case 0:
                valueRect = new Rect(position.x, position.y, position.width, position.height);
                EditorGUI.PropertyField(valueRect, property, GUIContent.none);
                if (property.objectReferenceValue == null)
                    return;
                break;
            case 1:
                valueRect = new Rect(position.x, position.y, position.width, position.height / 2);
                EditorGUI.PropertyField(valueRect, property, GUIContent.none);
                if (property.objectReferenceValue == null)
                    return;
                labelField = new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2);
                EditorGUI.LabelField(labelField, SetLabelText((GameEvent)property.objectReferenceValue));
                break;
            case 2:
                valueRect = new Rect(position.x, position.y, position.width - 50, position.height / 4.5f);
                EditorGUI.PropertyField(valueRect, property, GUIContent.none);
                if (property.objectReferenceValue == null)
                    return;
                buttonRect = new Rect(position.x + position.width - 50, position.y, 50, position.height / 4.5f);
                GUI.backgroundColor = Color.red;
                if (GUI.Button(buttonRect, "Set"))
                {
                    SetEventType(property);
                }
                GUI.backgroundColor = defaultBackgroundColor;
                helperBox = new Rect(position.x - EditorGUIUtility.labelWidth, position.y + position.height / 4.5f + position.height / 9, position.width + EditorGUIUtility.labelWidth, (2.5f * position.height / 4.5f));
                EditorGUI.HelpBox(helperBox, "Warning: Event not set yet!", MessageType.Warning);
                break;
            case 3:
                valueRect = new Rect(position.x, position.y, position.width, position.height / 5.5f);
                EditorGUI.PropertyField(valueRect, property, GUIContent.none);
                if (property.objectReferenceValue == null)
                    return;
                labelField = new Rect(position.x, position.y + position.height / 5.5f, position.width, position.height / 5.5f);
                EditorGUI.LabelField(labelField, SetLabelText((GameEvent)property.objectReferenceValue));
                helperBox = new Rect(position.x - EditorGUIUtility.labelWidth, position.y + 2*(position.height / 5.5f) + position.height / 11, position.width + EditorGUIUtility.labelWidth, (2.5f * position.height / 5.5f));
                DrawHelperBox(property, helperBox);
                break;
        }
        EditorGUI.indentLevel = indent;
        {
            /*

            if (property.objectReferenceValue == null)
            {
                valueRect = new Rect(position.x, position.y, position.width, position.height);
            }
            else
            {
                if (((GameEvent)property.objectReferenceValue).isSet)
                {   // Find out if Helperbox is showing shit && Find out if Typelabel is shown && GameEvent is set up if all true do this:
                    valueRect = new Rect(position.x, position.y, position.width, position.height / 5);
                }
                if (true) // TODO!!!
                {   // If Helperbox not shown but Typelabel is and Event is set up              // Event is setup perfectly
                    valueRect = new Rect(position.x, position.y, position.width, position.height / 2);
                }
                if (true) // TODO!!!
                {   // If Helperbox shown but TypeLabel not && event not set up
                    valueRect = new Rect(position.x, position.y, position.width - 50, position.height / 4);
                }
            }
            EditorGUI.PropertyField(valueRect, property, GUIContent.none);


            // TODO: New Dimensions in height
            helperBox = new Rect(position.x - EditorGUIUtility.labelWidth, position.y+(position.height/3)+position.height/12, position.width + EditorGUIUtility.labelWidth, (position.height/3)*2-2*(position.height / 12));

            if (property.objectReferenceValue != null)
            {
                GameEvent gameEvent = (GameEvent)property.objectReferenceValue;

                // Hier checken ob Eventtypen Übereinstimmen
                if(gameEvent.isSet)
                    gameEvent.isSet = CheckEventTypes(property);

                if (!gameEvent.isSet)
                {
                    GUI.backgroundColor = Color.red;
                }
                else
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUI.Button(buttonRect, "Set"))
                {
                    SetEventType(property);
                }

                GUI.backgroundColor = defaultBackgroundColor;

                SetLabelText(gameEvent, helperBox);
            }
            EditorGUI.indentLevel = indent;*/
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        switch (GetHeightState(property))
        {
            case 0: return base.GetPropertyHeight(property, label);
            case 1: return base.GetPropertyHeight(property, label) * 2;
            case 2: return base.GetPropertyHeight(property, label) * 4.5f;
            case 3: return base.GetPropertyHeight(property, label) * 5.5f;
            default: return base.GetPropertyHeight(property, label);
        }
    }

    private int GetHeightState(SerializedProperty property)
    {
        if (property.objectReferenceValue == null)
            return 0;
        else
        {
            if (((GameEvent)property.objectReferenceValue).isSet)
            {
                if (CheckEventTypes(property))
                {
                    return 1;
                }
                else
                {
                    return 3;
                }
            }
            else
                return 2;
        }
    }

    private void SetEventType(SerializedProperty prop)
    {
        MethodInfo[] methods = prop.serializedObject.targetObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        MethodInfo targetMethod = null;
        string targetMethodName = "Raise" + ((GameEvent)prop.objectReferenceValue).name;
        //Debug.Log("Searching for: " + targetMethodName + " | " + methods.Length + " Methods to search");
        foreach (MethodInfo method in methods)
        {
            string methodName = method.Name;
            if (methodName == targetMethodName)
                targetMethod = method;
        }

        Type[] types;
        if (targetMethod != null)
        {
            ParameterInfo[] parameters = targetMethod.GetParameters();
            types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }
        }
        else
        {
            types = new Type[0];
        }

        switch (types.Length)
        {
            case 0:
                ((GameEvent)prop.objectReferenceValue).SetType(null);
                break;
            case 1:
                ((GameEvent)prop.objectReferenceValue).SetType(types[0]);
                break;
            case 2:
                ((GameEvent)prop.objectReferenceValue).SetType(types[0], types[1]);
                break;
            case 3:
                ((GameEvent)prop.objectReferenceValue).SetType(types[0], types[1], types[2]);
                break;
            case 4:
                ((GameEvent)prop.objectReferenceValue).SetType(types[0], types[1], types[2], types[3]);
                break;
            default:
                Debug.LogWarning("Raiser Method has too many parameters! (" + types.Length + ") Aborting");
                tooMuchParameters = true;
                break;
        }
        EditorUtility.SetDirty(prop.objectReferenceValue);
    }
    
    private void UnsetEventTypes(SerializedProperty prop)
    {
        ((GameEvent)prop.objectReferenceValue).UnsetType();
    }

    private bool CheckEventTypes(SerializedProperty prop)
    {
        GameEvent _event = (GameEvent)prop.objectReferenceValue;
        MethodInfo[] methods = prop.serializedObject.targetObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public);
        MethodInfo targetMethod = null;
        string targetMethodName = "Raise" + ((GameEvent)prop.objectReferenceValue).name;
        //Debug.Log("Searching for: " + targetMethodName + " | " + methods.Length + " Methods to search");
        foreach (MethodInfo method in methods)
        {
            string methodName = method.Name;
            if (methodName == targetMethodName)
                targetMethod = method;
        }
        Type[] types;
        if (targetMethod != null)
        {
            ParameterInfo[] parameters = targetMethod.GetParameters();
            types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }
        }
        else
        {
            noMethodsFound = true;
            return false;
        }

        switch (types.Length)
        {
            case 0:
                if (_event.parameter1Type.type == null)
                    return true;
                else
                    return false;
            case 1:
                if (_event.parameter1Type.type == types[0] && _event.parameter2Type.type == null)
                    return true;
                else
                    return false;
            case 2:
                if (_event.parameter1Type.type == types[0] && _event.parameter2Type.type == types[1] && _event.parameter3Type.type == null)
                    return true;
                else
                    return false;
            case 3:
                if (_event.parameter1Type.type == types[0] && _event.parameter2Type.type == types[1] && _event.parameter3Type.type == types[2] && _event.parameter4Type.type == null)
                    return true;
                else
                    return false;
            case 4:
                if (_event.parameter1Type.type == types[0] && _event.parameter2Type.type == types[1] && _event.parameter3Type.type == types[2] && _event.parameter4Type.type == types[3])
                    return true;
                else
                    return false;
            default:
                tooMuchParameters = true;
                break;
        }
        return false;
    }

    private string SetLabelText(GameEvent _event)
    {
        string text;
        if (_event.parameter1Type.type == null)
            text = "Event has no parameters";
        else
        {
            text = TypeName(_event.parameter1Type.type);
            if (_event.parameter2Type.type != null)
                text += " | " + TypeName(_event.parameter2Type.type);
            if (_event.parameter3Type.type != null)
                text += " | " + TypeName(_event.parameter3Type.type);
            if (_event.parameter4Type.type != null)
                text += " | " + TypeName(_event.parameter4Type.type);
        }
        return text;
    }

    private void DrawHelperBox(SerializedProperty property, Rect position)
    {
        string text;
        if (!tooMuchParameters)
        {
            if (!noMethodsFound)
                text = "Error: Raise-Method parameters not matching Event parameters!";
            else
                text = "Error: No Raise-Method could be found!";
        }
        else
        {
            text = "Error: Too many parameters found!";
        }
        EditorGUI.HelpBox(position, text, MessageType.Error);
    }

    private string TypeName(Type t)
    {
        string fullName = t.Name;
        string shortenedName;
        for (int i = fullName.Length - 1; i > 0; i--)
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