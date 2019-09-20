using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Credits : MonoBehaviour {

    #region Variable Declarations
    [SerializeField] MusicTrack backgroundTrack;
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



    IEnumerator StartAudioNextFrame()
    {
        yield return null;
        AudioManager.Instance.StartTrack(backgroundTrack);
    }
}
