using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 
/// </summary>
public class MainMenu : MonoBehaviour
{

    #region Variable Declarations
    [SerializeField] GameObject playButton = null;
    [SerializeField] EventSystem eventSystem = null;
    
    AudioSource audioSource;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        audioSource = GetComponent<AudioSource>();
        eventSystem.SetSelectedGameObject(playButton);
	}

    private void Update() {
        
    }

    private void OnDisable()
    {
        AudioManager.Instance.StopPlaying();
    }
    #endregion



    #region Public Functions

    #endregion
}
