using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rewired;

public class Pause : MonoBehaviour
{
    public class PlayerRuleSet
    {
        public Player player;
        public List<ControllerMapLayoutManager.RuleSet> ruleSets = new List<ControllerMapLayoutManager.RuleSet>();
    }

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject resumeButton;
    [SerializeField] EventSystem eventSystem;

    [Header("References")]
    [SerializeField] GameEvent gamePausedEvent = null;
    [SerializeField] GameEvent gameResumedEvent = null;

    bool gameIsPaused;

    List<PlayerRuleSet> playerRuleSets = new List<PlayerRuleSet>();



    public bool GameIsPaused { get { return gameIsPaused; } }



	// Use this for initialization
    private void Update()
    {
        if (gameIsPaused && !optionsMenu.activeSelf && Input.GetButtonDown(Constants.INPUT_CANCEL))
        {
            resumeButton.GetComponent<Button>().onClick.Invoke();
        }

        if (Input.GetButtonDown(Constants.INPUT_ESCAPE))
        {
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (gameIsPaused && !optionsMenu.activeSelf && Input.GetButtonDown(Constants.INPUT_CANCEL))
        {
            resumeButton.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        foreach (Player player in ReInput.players.Players)
        {
            playerRuleSets.Add(new PlayerRuleSet { player = player, ruleSets = player.controllers.maps.layoutManager.ruleSets });
            player.controllers.maps.layoutManager.ruleSets.Clear();
            player.controllers.maps.layoutManager.ruleSets.Add(ReInput.mapping.GetControllerMapLayoutManagerRuleSetInstance("RuleSetMenu"));
            player.controllers.maps.layoutManager.Apply();
        }

        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(resumeButton);

        gameIsPaused = true;

        RaiseGamePaused();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        foreach (Player player in ReInput.players.Players)
        {
            player.controllers.maps.layoutManager.ruleSets.Clear();
            player.controllers.maps.layoutManager.ruleSets.AddRange(playerRuleSets.Find(x => x.player == player).ruleSets);
            player.controllers.maps.layoutManager.Apply();
        }

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(null);

        gameIsPaused = false;

        RaiseGameResumed();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.Instance.LoadMainMenu();
    }

    public void ExitGame()
    {
        SceneManager.Instance.ExitGame();
    }

    private void RaiseGamePaused()
    {
        gamePausedEvent.Raise(this);
    }
    
    private void RaiseGameResumed()
    {
        gameResumedEvent.Raise(this);
    }
}
