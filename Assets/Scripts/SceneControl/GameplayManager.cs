using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class GameplayManager : LevelManager 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("Game Properties")]
    [SerializeField] GameSettings gameSettings = null;

    [Space]
    [SerializeField] Points points = null;

    // Private
    float intensifyTimer = 0f;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Start () 
	{
		
	}

    private void Update()
    {
        intensifyTimer += Time.deltaTime;

        HandleIntensify();
    }
    #endregion



    #region Public Functions
    /// <summary>
    /// Listens for LevelStarted
    /// </summary>
    public void ResetIntensifyTimer()
    {
        intensifyTimer = 0f;
    }
    #endregion



    #region Inherited Functions
    protected override void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        throw new System.NotImplementedException();
    }

    protected override void RaiseLevelLoaded(float levelStartDelay)
    {
        throw new System.NotImplementedException();
    }

    protected override void RaiseLevelStarted()
    {
        throw new System.NotImplementedException();
    }
    #endregion



    #region Private Functions
    void HandleIntensify()
    {
        if (intensifyTimer >= gameSettings.IntensifyTime)
        {
            // Set new pointLeadToWin
            points.PointLeadToWin = Mathf.RoundToInt(points.PointLeadToWin * (1 - gameSettings.IntensifyAmount));

            intensifyTimer = 0f;
        }
    }
    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}

