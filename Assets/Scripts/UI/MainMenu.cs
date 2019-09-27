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
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start()
    {
        eventSystem.SetSelectedGameObject(playButton);
	}

    private void OnDisable()
    {
        AudioManager.Instance.StopPlaying();
    }
    #endregion



    #region Public Functions
    public void LoadFirstLevel()
    {
        SceneManager.Instance.LoadNextScene();
    }

    public void LoadTutorial()
    {
        SceneManager.Instance.LoadTutorial();
    }
    #endregion
}
