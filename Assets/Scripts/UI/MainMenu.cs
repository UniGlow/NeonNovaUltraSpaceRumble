using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 
/// </summary>
public class MainMenu : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] GameObject playButton;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        GameObject.FindObjectOfType<EventSystem>().SetSelectedGameObject(playButton);
	}
	
	private void Update() {
		
	}
	#endregion
	
	
	
	#region Private Functions
	#endregion
}
