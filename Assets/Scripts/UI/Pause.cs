using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject resumeButton;
    [SerializeField] EventSystem eventSystem;

    bool gameIsPaused;
    

    private void Update() {
        if (gameIsPaused && !optionsMenu.activeSelf && Input.GetButtonDown(Constants.INPUT_CANCEL)) {
            resumeButton.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void PauseGame() {
        Time.timeScale = 0;

        Rumble.Instance.StopAllRumble();

        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(resumeButton);

        gameIsPaused = true;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(null);

        gameIsPaused = false;
    }

    public void ReturnToMainMenu() {
        SceneManager.Instance.LoadMainMenu();
    }
}
