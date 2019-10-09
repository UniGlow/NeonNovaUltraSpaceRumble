using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class EditorLevelStarter : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [SerializeField] PlayerConfig bossPlayerConfig = null;
    [SerializeField] PlayerConfig hero1PlayerConfig = null;
    [SerializeField] PlayerConfig hero2PlayerConfig = null;
    [SerializeField] PlayerConfig hero3PlayerConfig = null;
    [Space]
    [SerializeField] ColorSet colorSet = null;
    [Space]
    [SerializeField] Ability ability1 = null;
    [SerializeField] Ability ability2 = null;
    [SerializeField] Ability ability3 = null;
    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    protected void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    protected void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    #endregion



    #region Public Functions

    #endregion



    #region Private Functions
    private void OnLevelFinishedLoading(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
#if UNITY_EDITOR
        if (!GameManager.Instance.IsInitialized)
        {
            SetupPlayers();
        }
#endif
    }

    private void SetupPlayers()
    {

        int playerCount = UpdatePlayerCount();
        // Set playerNumbers depending on amount of human players
        switch (playerCount)
        {
            case 1:
                bossPlayerConfig.Initialize(1, Faction.Boss, colorSet.GetRandomColor(), false);
                hero1PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color1, true);
                hero1PlayerConfig.ability = ability2;
                hero2PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color2, true);
                hero2PlayerConfig.ability = ability1;
                hero3PlayerConfig.Initialize(4, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.ability = ability3;
                break;

            case 2:
                hero1PlayerConfig.Initialize(1, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = ability2;
                hero2PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = ability1;
                hero3PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.ability = ability3;
                bossPlayerConfig.Initialize(4, Faction.Boss, colorSet.GetRandomColor(), true);
                break;

            case 3:
                hero1PlayerConfig.Initialize(1, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = ability2;
                hero2PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = ability1;
                hero3PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.ability = ability3;
                bossPlayerConfig.Initialize(4, Faction.Boss, colorSet.GetRandomColor(), true);
                break;

            case 4:
                bossPlayerConfig.Initialize(1, Faction.Boss, colorSet.GetRandomColor(), false);
                hero1PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = ability2;
                hero2PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = ability1;
                hero3PlayerConfig.Initialize(4, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.ability = ability3;
                break;

            default:
                break;
        }

        GameManager.Instance.activeColorSet = colorSet;
        GameManager.Instance.IsInitialized = true;
    }

    public int UpdatePlayerCount()
    {
        int playerCount = 0;
        string[] joystickNames = Input.GetJoystickNames();
        foreach (string name in joystickNames)
        {
            if (name != "")
            {
                playerCount++;
            }
        }
        if (playerCount == 0)
            playerCount = 1;
        return playerCount;
    }
    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}

