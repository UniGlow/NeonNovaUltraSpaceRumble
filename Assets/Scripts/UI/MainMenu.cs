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
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject howToPlayOne;
    [SerializeField] GameObject howToPlayTwo;

    int howToPlay;
    EventSystem eventSystem;
    AudioSource audioSource;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        audioSource = GetComponent<AudioSource>();
        eventSystem = GameObject.FindObjectOfType<EventSystem>();
        eventSystem.SetSelectedGameObject(playButton);
	}

    private void Update() {
        if (howToPlay == 1) {
            eventSystem.SetSelectedGameObject(null);
            howToPlayOne.SetActive(true);
            howToPlay = 2;
        } else if (howToPlay == 2 && Input.GetButtonDown(Constants.INPUT_SUBMIT)) {
            howToPlayOne.SetActive(false);
            howToPlayTwo.SetActive(true);
            audioSource.Play();
            howToPlay = 3;
        }
        else if (howToPlay == 3 && Input.GetButtonDown(Constants.INPUT_SUBMIT)) {
            howToPlayTwo.SetActive(false);
            eventSystem.SetSelectedGameObject(playButton);
            audioSource.Play();
            howToPlay = 0;
        }
    }
    #endregion



    #region Public Functions
    public void SetHowToPlay(int status) {
        howToPlay = status;
    }
    #endregion
}
