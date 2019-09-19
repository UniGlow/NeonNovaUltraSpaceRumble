using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{

    GameObject pauseMenu,
               mainMenu,
               optionsMenu,
               resumeButton;
    EventSystem eventSystem;
    bool gameIsPaused;
    public bool GameIsPaused { get { return gameIsPaused; } }

	// Use this for initialization
	void Start () {
        pauseMenu = transform.Find("PauseMenu").gameObject;
        mainMenu = pauseMenu.transform.Find("MainMenu").gameObject;
        resumeButton = mainMenu.transform.Find("ResumeButton").gameObject;
        optionsMenu = pauseMenu.transform.Find("OptionsMenu").gameObject;
        eventSystem = transform.parent.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Update() {
        if (gameIsPaused && !optionsMenu.activeSelf && Input.GetButtonDown(Constants.INPUT_CANCEL)) {
            resumeButton.GetComponent<Button>().onClick.Invoke();
        }

        if (Input.GetButtonDown(Constants.INPUT_ESCAPE)) {
            if (gameIsPaused) {
                ResumeGame();
            }
            else {
                PauseGame();
            }
        }
    }

    public void PauseGame() {
        Time.timeScale = 0;

        Rumble.Instance.StopAllRumble();

        GameObject.FindObjectOfType<HomingMissile>().PauseMissile(true);

        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(resumeButton);

        gameIsPaused = true;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;

        GameObject.FindObjectOfType<HomingMissile>().PauseMissile(false);

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(null);

        gameIsPaused = false;
    }

    public void ReturnToMainMenu() {
        SceneManager.Instance.LoadLevel("MainMenu");
    }
}
