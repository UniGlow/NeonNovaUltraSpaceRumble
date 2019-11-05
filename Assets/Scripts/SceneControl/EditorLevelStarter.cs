#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public abstract class EditorLevelStarter : MonoBehaviour 
{
    #region Variable Declarations
    public static EditorLevelStarter Instance = null;
    // Serialized Fields
    
    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    protected void Awake()
    {
        if (Instance == null)
            Instance = this;
        InheritedAwake();
    }
    #endregion



    #region Public Functions

    #endregion



    #region Private Functions
    public abstract void Initialize();
    protected virtual void InheritedAwake()
    {

    }
    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}
#endif