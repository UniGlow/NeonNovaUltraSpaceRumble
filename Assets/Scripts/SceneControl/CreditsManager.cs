using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class CreditsManager : LevelManager
{

    #region Variable Declarations
    [SerializeField] MusicTrack backgroundTrack = null;
    [SerializeField] Points points = null;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        StartCoroutine(StartAudioNextFrame());
	}
	
	private void Update() {
        if (Input.GetButtonDown(Constants.INPUT_ESCAPE) || Input.GetButtonDown(Constants.INPUT_CANCEL)) {
            SceneManager.Instance.LoadMainMenu();
        }
	}
    #endregion



    #region Inherited Functions
    protected override void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        points.ResetPoints(true);

        Invoke("RaiseLevelStarted", 3f);
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



    IEnumerator StartAudioNextFrame()
    {
        yield return null;
        AudioManager.Instance.StartTrack(backgroundTrack);
    }
}
