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
        PlayerSetup.SetupPlayers(playerCount, bossPlayerConfig, hero1PlayerConfig, hero2PlayerConfig, hero3PlayerConfig, ability1, ability2, ability3, colorSet);
    }

    /// <summary>
    /// TODO: With Rewired, this function should be called from a static helper class (currently copy-pasted code from SirAlfred)
    /// </summary>
    /// <returns></returns>
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

