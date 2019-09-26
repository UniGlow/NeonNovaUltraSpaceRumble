using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for every LevelManager in the game.
/// A LevelManager takes care of the flow of a specific type of level (Intro, Credits, Menu, gameplay, ...)
/// </summary>
public abstract class LevelManager : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [SerializeField] protected GameEvent levelLoadedEvent = null;
    [SerializeField] protected GameEvent levelStartedEvent = null;

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    protected void OnEnable () 
	{
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;

        InheritedOnEnable();
    }

    protected void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLevelFinishedLoading;

        InheritedOnDisable();
    }
    #endregion



    #region Public Functions

    #endregion



    #region Protected Functions
    protected abstract void OnLevelFinishedLoading(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode);

    protected abstract void RaiseLevelLoaded(float levelStartDelay);

    protected abstract void RaiseLevelStarted();

    protected virtual void InheritedOnEnable() { }

    protected virtual void InheritedOnDisable() { }
    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}

