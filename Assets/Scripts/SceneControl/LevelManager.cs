using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for every LevelManager in the game.
/// A LevelManager takes care of the flow of a specific type of level (Intro, Credits, Menu, gameplay, ...)m
/// </summary>
public abstract class LevelManager : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("Level Manager Variables")]
    [SerializeField] protected GameEvent levelInitializedEvent = null;
    [SerializeField] protected GameEvent levelStartedEvent = null;

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    protected void OnEnable () 
	{
        //UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;

        InheritedOnEnable();
    }

    protected void OnDisable()
    {
        //UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLevelFinishedLoading;

        InheritedOnDisable();
    }
    #endregion



    #region Public Functions

    #endregion



    #region Protected Functions
    //protected virtual void OnLevelFinishedLoading(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) { }

    protected virtual void RaiseLevelInitialized(float levelStartDelay)
    {
        levelInitializedEvent.Raise(this, levelStartDelay);
    }

    protected virtual void RaiseLevelStarted()
    {
        levelStartedEvent.Raise(this);
    }

    protected virtual void InheritedOnEnable() { }

    protected virtual void InheritedOnDisable() { }
    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}

