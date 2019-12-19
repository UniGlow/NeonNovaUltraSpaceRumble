using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

/// <summary>
/// 
/// </summary>
public class CreditsManager : LevelManager
{

    #region Variable Declarations
    [SerializeField] MusicTrack backgroundTrack = null;
    [SerializeField] Points points = null;

    List<InputHelper.PlayerRuleSet> ruleSets = new List<InputHelper.PlayerRuleSet>();
    #endregion



    #region Unity Event Functions
    private void Start()
    {
        StartCoroutine(StartAudioNextFrame());
        ruleSets = InputHelper.ChangeRuleSetForAllPlayers(RewiredConsts.LayoutManagerRuleSet.RULESETMENU);
    }
	
	private void Update()
    {
        if (InputHelper.GetButtonDown(RewiredConsts.Action.UICANCEL) || InputHelper.GetButtonDown(RewiredConsts.Action.UISUBMIT))
        {
            SceneManager.Instance.LoadMainMenu();
        }
	}

    private void OnDisable()
    {
        InputHelper.ChangeRuleSetForPlayers(ruleSets);
    }
    #endregion



    #region Public Functions
    public void Initialize()
    {
        points.ResetLevelPoints(true);
        RaiseLevelInitialized(0f);
        Invoke("RaiseLevelStarted", 0.01f);
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



    IEnumerator StartAudioNextFrame()
    {
        yield return null;
        AudioManager.Instance.StartTrack(backgroundTrack);
    }
}
