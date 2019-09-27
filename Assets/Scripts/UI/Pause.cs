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
        public Rewired.Player player;
        public List<ControllerMapLayoutManager.RuleSet> ruleSets = new List<ControllerMapLayoutManager.RuleSet>();
    }

    GameObject pauseMenu,
               mainMenu,
               optionsMenu,
               resumeButton;
    EventSystem eventSystem;
    bool gameIsPaused;
    List<PlayerRuleSet> playerRuleSets = new List<PlayerRuleSet>();



    public bool GameIsPaused { get { return gameIsPaused; } }



	// Use this for initialization
	void Start ()
    {
        pauseMenu = transform.Find("PauseMenu").gameObject;
        mainMenu = pauseMenu.transform.Find("MainMenu").gameObject;
        resumeButton = mainMenu.transform.Find("ResumeButton").gameObject;
        optionsMenu = pauseMenu.transform.Find("OptionsMenu").gameObject;
        eventSystem = transform.parent.Find("Rewired Event System").GetComponent<EventSystem>();
    }

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
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        foreach (Rewired.Player player in ReInput.players.Players)
        {
            playerRuleSets.Add(new PlayerRuleSet { player = player, ruleSets = player.controllers.maps.layoutManager.ruleSets });
            player.controllers.maps.layoutManager.ruleSets.Clear();
            player.controllers.maps.layoutManager.ruleSets.Add(ReInput.mapping.GetControllerMapLayoutManagerRuleSetInstance("RuleSetMenu"));
            player.controllers.maps.layoutManager.Apply();
        }

        GameObject.FindObjectOfType<HomingMissile>().PauseMissile(true);

        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(resumeButton);

        gameIsPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        foreach (Rewired.Player player in ReInput.players.Players)
        {
            player.controllers.maps.layoutManager.ruleSets.Clear();
            player.controllers.maps.layoutManager.ruleSets.AddRange(playerRuleSets.Find(x => x.player == player).ruleSets);
            player.controllers.maps.layoutManager.Apply();
        }

        GameObject.FindObjectOfType<HomingMissile>().PauseMissile(false);

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(null);

        gameIsPaused = false;
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.LoadLevel("MainMenu");
    }
}
