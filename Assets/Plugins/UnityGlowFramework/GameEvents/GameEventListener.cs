using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;
using System;

[Serializable]
public class LinkedMethod
{
    public Component component;
    public int methodID;
    public string methodName;
    public int chosenID;
    public string[] methodNames;
    public GameObject gameObject;

    public LinkedMethod()
    {
        component = null;
        methodID = -1;
        methodName = null;
        chosenID = -1;
        methodNames = new string[0];
    }

    public void Invoke<T>(T param1)
    {
        component.GetType().GetMethods()[methodID].Invoke(
            component,
            new object[] { param1 });
    }
    public void Invoke<T, U>(T param1, U param2)
    {
        component.GetType().GetMethods()[methodID].Invoke(
            component,
            new object[] { param1, param2 });
    }
    public void Invoke<T, U, O>(T param1, U param2, O param3)
    {
        component.GetType().GetMethods()[methodID].Invoke(
            component,
            new object[] { param1, param2, param3 });
    }
    public void Invoke<T, U, O, L>(T param1, U param2, O param3, L param4)
    {
        component.GetType().GetMethods()[methodID].Invoke(
            component,
            new object[] { param1, param2, param3, param4 });
    }
}



public class GameEventListener : MonoBehaviour
{

    public GameEvent gameEvent;
    public UnityEvent response;
    
    public List<LinkedMethod> linkedMethods;

    public bool ignoreEventParameters;

    public GameEvent Event { get { return gameEvent; } set { gameEvent = value; } }
    


    public void OnRaise()
    {
        for (int i = 0; i < response.GetPersistentEventCount(); i++)
        {
            response.GetPersistentMethodName(i);
            if (response.GetPersistentMethodName(i) == "")
            {
                Debug.LogWarning("Tried to raise unset method through GameEvent: " + gameEvent.name + " on Listener: " + gameObject);
                return;
            }
        }

        response.Invoke();
    }

    public void OnRaise<T>(T firstParameter)
    {
        if (ignoreEventParameters)
        {
            OnRaise();
            return;
        }

        linkedMethods.ForEach((LinkedMethod lm) =>
        {
            if(lm.methodID >= 0)
                lm.Invoke(firstParameter);
            else
                Debug.LogWarning("Tried to raise unset method through GameEvent: " + gameEvent.name + " on Listener: " + gameObject);
        });
    }

    public void OnRaise<T, U>(T firstParameter, U secondParameter)
    {
        if (ignoreEventParameters)
        {
            OnRaise();
            return;
        }

        linkedMethods.ForEach((LinkedMethod lm) =>
        {
            if (lm.methodID >= 0)
                lm.Invoke(firstParameter, secondParameter);
            else
                Debug.LogWarning("Tried to raise unset method through GameEvent: " + gameEvent.name + " on Listener: " + gameObject);
        });
    }

    public void OnRaise<T,U,O>(T firstParameter, U secondParameter, O thirdParameter)
    {
        if (ignoreEventParameters)
        {
            OnRaise();
            return;
        }

        linkedMethods.ForEach((LinkedMethod lm) =>
        {
            if (lm.methodID >= 0)
                lm.Invoke(firstParameter, secondParameter, thirdParameter);
            else
                Debug.LogWarning("Tried to raise unset method through GameEvent: " + gameEvent.name + " on Listener: " + gameObject);
        });
    }

    public void OnRaise<T, U, O, L>(T firstParameter, U secondParameter, O thirdParameter, L fourthParameter)
    {
        if (ignoreEventParameters)
        {
            OnRaise();
            return;
        }

        linkedMethods.ForEach((LinkedMethod lm) =>
        {
            if (lm.methodID >= 0)
                lm.Invoke(firstParameter, secondParameter, thirdParameter, fourthParameter);
            else
                Debug.LogWarning("Tried to raise unset method through GameEvent: " + gameEvent.name + " on Listener: " + gameObject);
        });
    }

    private void OnEnable()
    {
        if (gameEvent != null) gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (gameEvent != null) gameEvent.UnregisterListener(this);
    }
}
