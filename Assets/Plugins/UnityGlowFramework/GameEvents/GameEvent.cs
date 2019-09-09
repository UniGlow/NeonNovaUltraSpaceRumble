using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Game Event")]
public class GameEvent : ScriptableObject
{
    [System.Serializable]
    public class EventRaise
    {
        public UnityEngine.Object EventRaiser;
        public float TimeStamp;
        public bool raisedSuccessfull;
    }

    #region Variable Declarations
    [TextArea(2, 5)]
    public string description;

    [HideInInspector] public SerializableType parameter1Type = null;
    [HideInInspector] public SerializableType parameter2Type = null;
    [HideInInspector] public SerializableType parameter3Type = null;
    [HideInInspector] public SerializableType parameter4Type = null;

    List<GameEventListener> listeners = new List<GameEventListener>();
    List<EventRaise> raisedEvents = new List<EventRaise>();
    #endregion



    #region Variables for the Property Drawer
    [HideInInspector] public bool isSet;
    [HideInInspector] public object editorParameter1;
    [HideInInspector] public object editorParameter2;
    [HideInInspector] public object editorParameter3;
    [HideInInspector] public object editorParameter4;
    #endregion



    #region Unity Event Functions
    private void OnEnable()
    {
        raisedEvents.Clear();
    }
    #endregion



    #region Raise Functions
    public void Raise(UnityEngine.Object eventRaiser)
    {
        // Create EventRaise for Debug
        EventRaise eventRaised = new EventRaise
        {
            EventRaiser = eventRaiser,
            TimeStamp = Time.time
        };
        AddRaisedEvent(eventRaised);

        // Check for parameter types
        if (parameter1Type.type != null || parameter2Type.type != null || parameter3Type.type != null || parameter4Type.type != null)
        {
            Debug.LogError(BuildErrorString());
            eventRaised.raisedSuccessfull = false;
            return;
        }

        // Call functions on listeners
        listeners.ForEach((GameEventListener listener) =>
        {
            listener.OnRaise();
        });

        eventRaised.raisedSuccessfull = true;
    }

    public void Raise<T>(UnityEngine.Object eventRaiser, T firstParameter)
    {
        // Create EventRaise for Debug
        EventRaise eventRaised = new EventRaise
        {
            EventRaiser = eventRaiser,
            TimeStamp = Time.time
        };
        AddRaisedEvent(eventRaised);

        // Check for parameter types
        if ((firstParameter != null && parameter1Type.type != firstParameter.GetType() && !firstParameter.GetType().IsSubclassOf(parameter1Type.type)) 
            || parameter2Type.type != null 
            || parameter3Type.type != null 
            || parameter4Type.type != null)
        {
            Debug.LogError(BuildErrorString(firstParameter));
            eventRaised.raisedSuccessfull = false;
            return;
        }

        // Call functions on listeners
        listeners.ForEach((GameEventListener rel) =>
        {
            rel.OnRaise(firstParameter);
        });

        eventRaised.raisedSuccessfull = true;
    }

    public void Raise<T,U>(UnityEngine.Object eventRaiser, T firstParameter, U secondParameter)
    {
        // Create EventRaise for Debug
        EventRaise eventRaised = new EventRaise
        {
            EventRaiser = eventRaiser,
            TimeStamp = Time.time
        };
        AddRaisedEvent(eventRaised);

        // Check for parameter types
        if ((firstParameter != null && parameter1Type.type != firstParameter.GetType() && !firstParameter.GetType().IsSubclassOf(parameter1Type.type))
            || (secondParameter != null && parameter2Type.type != secondParameter.GetType() && !secondParameter.GetType().IsSubclassOf(parameter2Type.type))
            || parameter3Type.type != null 
            || parameter4Type.type != null)
        {
            Debug.LogError(BuildErrorString(firstParameter, secondParameter));
            eventRaised.raisedSuccessfull = false;
            return;
        }

        // Call functions on listeners
        listeners.ForEach((GameEventListener rel) =>
        {
            rel.OnRaise(firstParameter, secondParameter);
        });

        eventRaised.raisedSuccessfull = true;
    }

    public void Raise<T, U, O>(UnityEngine.Object eventRaiser, T firstParameter, U secondParameter, O thirdParameter)
    {
        // Create EventRaise for Debug
        EventRaise eventRaised = new EventRaise
        {
            EventRaiser = eventRaiser,
            TimeStamp = Time.time
        };
        AddRaisedEvent(eventRaised);

        // Check for parameter types
        if ((firstParameter != null && parameter1Type.type != firstParameter.GetType() && !firstParameter.GetType().IsSubclassOf(parameter1Type.type))
            || (secondParameter != null && parameter2Type.type != secondParameter.GetType() && !secondParameter.GetType().IsSubclassOf(parameter2Type.type))
            || (thirdParameter != null && parameter3Type.type != thirdParameter.GetType() && !thirdParameter.GetType().IsSubclassOf(parameter3Type.type))
            || parameter4Type.type != null)
        {
            Debug.LogError(BuildErrorString(firstParameter, secondParameter, thirdParameter));
            eventRaised.raisedSuccessfull = false;
            return;
        }

        // Call functions on listeners
        listeners.ForEach((GameEventListener rel) =>
        {
            rel.OnRaise(firstParameter, secondParameter, thirdParameter);
        });

        eventRaised.raisedSuccessfull = true;
    }

    public void Raise<T, U, O, L>(UnityEngine.Object eventRaiser, T firstParameter, U secondParameter, O thirdParameter, L fourthParameter)
    {
        // Create EventRaise for Debug
        EventRaise eventRaised = new EventRaise
        {
            EventRaiser = eventRaiser,
            TimeStamp = Time.time
        };
        AddRaisedEvent(eventRaised);

        // Check for parameter types
        if ((firstParameter != null && parameter1Type.type != firstParameter.GetType() && !firstParameter.GetType().IsSubclassOf(parameter1Type.type))
            || (secondParameter != null && parameter2Type.type != secondParameter.GetType() && !secondParameter.GetType().IsSubclassOf(parameter2Type.type))
            || (thirdParameter != null && parameter3Type.type != thirdParameter.GetType() && !thirdParameter.GetType().IsSubclassOf(parameter3Type.type))
            || (fourthParameter != null && parameter4Type.type != fourthParameter.GetType() && !fourthParameter.GetType().IsSubclassOf(parameter4Type.type)))
        {
            Debug.LogError(BuildErrorString(firstParameter, secondParameter, thirdParameter, fourthParameter));
            eventRaised.raisedSuccessfull = false;
            return;
        }

        // Call functions on listeners
        listeners.ForEach((GameEventListener rel) =>
        {
            rel.OnRaise(firstParameter, secondParameter, thirdParameter, fourthParameter);
        });

        eventRaised.raisedSuccessfull = true;
    }

    [ExecuteInEditMode]
    public void SetType(Type t1)
    {
        parameter1Type = new SerializableType(t1);
        parameter2Type = new SerializableType(null);
        parameter3Type = new SerializableType(null);
        parameter4Type = new SerializableType(null);
        isSet = true;
        //LogTypes();
    }
    [ExecuteInEditMode]
    public void SetType(Type t1, Type t2)
    {
        parameter1Type = new SerializableType(t1);
        parameter2Type = new SerializableType(t2);
        parameter3Type = new SerializableType(null);
        parameter4Type = new SerializableType(null);
        isSet = true;
        //LogTypes();
    }
    [ExecuteInEditMode]
    public void SetType(Type t1, Type t2, Type t3)
    {
        parameter1Type = new SerializableType(t1);
        parameter2Type = new SerializableType(t2);
        parameter3Type = new SerializableType(t3);
        parameter4Type = new SerializableType(null);
        isSet = true;
        //LogTypes();
    }
    [ExecuteInEditMode]
    public void SetType(Type t1, Type t2, Type t3, Type t4)
    {
        parameter1Type = new SerializableType(t1);
        parameter2Type = new SerializableType(t2);
        parameter3Type = new SerializableType(t3);
        parameter4Type = new SerializableType(t4);
        isSet = true;
        //LogTypes();
    }
    [ExecuteInEditMode]
    public void UnsetType()
    {
        parameter1Type = new SerializableType(null);
        parameter2Type = new SerializableType(null);
        parameter3Type = new SerializableType(null);
        parameter4Type = new SerializableType(null);
        isSet = false;
        //LogTypes();
    }
    #endregion



    #region Listener Functions
    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if(listeners.Contains(listener))
            listeners.Remove(listener);
    }
    #endregion



    #region Private Functions
    void AddRaisedEvent(EventRaise eventRaise)
    {
        raisedEvents.Add(eventRaise);

        if (raisedEvents.Count > 1000)
        {
            raisedEvents.RemoveAt(0);
        }
    }

    void LogTypes()
    {
        string log = "Types of " + this.name + " are now: ";
        if (parameter1Type.type != null)
            log += parameter1Type.type.ToString();
        else
            log += "null";
        log += " + ";
        if (parameter2Type.type != null)
            log += parameter2Type.type.ToString();
        else
            log += "null";
        log += " + ";
        if (parameter3Type.type != null)
            log += parameter3Type.type.ToString();
        else
            log += "null";
        log += " + ";
        if (parameter4Type.type != null)
            log += parameter4Type.type.ToString();
        else
            log += "null";
        Debug.Log(log);
    }

    string BuildErrorString()
    {
        string print = "Tried to raise GameEvent with incorrect parameters. Given: ";
        print += "None Needed: ";
        print += (parameter1Type.type == null) ? "" : parameter1Type.type.ToString() + " ";
        print += (parameter2Type.type == null) ? "" : parameter2Type.type.ToString() + " ";
        print += (parameter3Type.type == null) ? "" : parameter3Type.type.ToString() + " ";
        print += (parameter4Type.type == null) ? "" : parameter4Type.type.ToString() + " ";

        return print;
    }

    string BuildErrorString<T>(T firstParameter)
    {
        string print = "Tried to raise GameEvent with incorrect parameters. Given: ";
        print += firstParameter.GetType().ToString() + " Needed: ";
        print += (parameter1Type.type == null) ? "" : parameter1Type.type.ToString() + " ";
        print += (parameter2Type.type == null) ? "" : parameter2Type.type.ToString() + " ";
        print += (parameter3Type.type == null) ? "" : parameter3Type.type.ToString() + " ";
        print += (parameter4Type.type == null) ? "" : parameter4Type.type.ToString() + " ";

        return print;
    }

    string BuildErrorString<T, U>(T firstParameter, U secondParameter)
    {
        string print = "Tried to raise GameEvent with incorrect parameters. Given: ";
        print += firstParameter.GetType().ToString() + " ";
        print += secondParameter.GetType().ToString() + " Needed: ";
        print += (parameter1Type.type == null) ? "" : parameter1Type.type.ToString() + " ";
        print += (parameter2Type.type == null) ? "" : parameter2Type.type.ToString() + " ";
        print += (parameter3Type.type == null) ? "" : parameter3Type.type.ToString() + " ";
        print += (parameter4Type.type == null) ? "" : parameter4Type.type.ToString() + " ";

        return print;
    }

    string BuildErrorString<T, U, O>(T firstParameter, U secondParameter, O thirdParameter)
    {
        string print = "Tried to raise GameEvent with incorrect parameters. Given: ";
        print += firstParameter.GetType().ToString() + " ";
        print += secondParameter.GetType().ToString() + " ";
        print += thirdParameter.GetType().ToString() + " Needed: ";
        print += (parameter1Type.type == null) ? "" : parameter1Type.type.ToString() + " ";
        print += (parameter2Type.type == null) ? "" : parameter2Type.type.ToString() + " ";
        print += (parameter3Type.type == null) ? "" : parameter3Type.type.ToString() + " ";
        print += (parameter4Type.type == null) ? "" : parameter4Type.type.ToString() + " ";

        return print;
    }

    string BuildErrorString<T, U, O, L>(T firstParameter, U secondParameter, O thirdParameter, L fourthParameter)
    {
        string print = "Tried to raise GameEvent with incorrect parameters. Given: ";
        print += firstParameter.GetType().ToString() + " ";
        print += secondParameter.GetType().ToString() + " ";
        print += thirdParameter.GetType().ToString() + " ";
        print += fourthParameter.GetType().ToString() + " Needed: ";
        print += (parameter1Type.type == null) ? "" : parameter1Type.type.ToString() + " ";
        print += (parameter2Type.type == null) ? "" : parameter2Type.type.ToString() + " ";
        print += (parameter3Type.type == null) ? "" : parameter3Type.type.ToString() + " ";
        print += (parameter4Type.type == null) ? "" : parameter4Type.type.ToString() + " ";

        return print;
    }
    #endregion
}
