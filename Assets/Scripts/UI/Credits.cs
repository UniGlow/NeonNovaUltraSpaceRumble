using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Credits : MonoBehaviour {
	
	#region Variable Declarations
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        AudioManager.Instance.StartBackgroundTrack();
	}
	
	private void Update() {
        if (Input.GetButtonDown(Constants.INPUT_ESCAPE) || Input.GetButtonDown(Constants.INPUT_CANCEL)) {
            GameManager.Instance.LoadLevel("MainMenu");
        }
	}
	#endregion
	
	
	
	#region Private Functions
	#endregion
}
