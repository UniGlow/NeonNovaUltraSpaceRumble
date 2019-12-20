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
    bool waitingForInputToContinue = false;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Start () 
	{
        points.PointLeadToWin = gameSettings.PointLeadToWin;
        SceneManager.Instance.ManualContine = gameSettings.UseEndScores;

        waitingForInputToContinue = false;
	}

    private void Update()
    {
        intensifyTimer += Time.deltaTime;

        HandleIntensify();

        //TODO: Delay
        if (waitingForInputToContinue)
        {
            if((points.BossWins > gameSettings.BestOf / 2) || (points.HeroWins > gameSettings.BestOf / 2))
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.Instance.LoadCredits();
                }
            }
            else
            {
                if (InputHelper.GetButtonDown(RewiredConsts.Action.READY_UP))
                {
                    SceneManager.Instance.LoadNextLevel(1f);
                }
            }
        }
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

    public void InitializeLevel()
    {
        RaiseLevelInitialized(4f);
        points.ResetLevelPoints(false);

        StartCoroutine(InitializeLevelCoroutine());
    }

    public void WaitForContinueInput()
    {
        Time.timeScale = 0f;
        if (gameSettings.UseEndScores)
            StartCoroutine(SetWaitForInputDelayed());
    }
    #endregion



    #region Inherited Functions
    protected override void RaiseLevelInitialized(float levelStartDelay)
    {
        base.RaiseLevelInitialized(levelStartDelay);
    }

    protected override void RaiseLevelStarted()
    {
        base.RaiseLevelStarted();
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
    IEnumerator InitializeLevelCoroutine()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.StartRandomTrack();

        yield return new WaitForSeconds(3f);

        RaiseLevelStarted();
    }

    IEnumerator SetWaitForInputDelayed()
    {
        yield return new WaitForSecondsRealtime(gameSettings.DelayForButtonPrompt);
        waitingForInputToContinue = true;
    }
    #endregion
}

