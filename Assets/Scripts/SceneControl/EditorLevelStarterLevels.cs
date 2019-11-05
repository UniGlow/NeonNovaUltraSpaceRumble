#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class EditorLevelStarterLevels : EditorLevelStarter
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
    
    #endregion



    #region Public Functions

    #endregion



    #region Private Functions
    public override void Initialize()
    {
        if (!GameManager.Instance.IsInitialized)
        {
            if(UnityEngine.SceneManagement.SceneManager.sceneCount <= 1)
                SceneManager.Instance.LoadUIAdditive();
            SetupPlayers();
        }
    }

    private void SetupPlayers()
    {
        int playerCount = InputHelper.UpdatePlayerCount();
        // Set playerNumbers depending on amount of human players
        PlayerSetup.SetupPlayers(playerCount, bossPlayerConfig, hero1PlayerConfig, hero2PlayerConfig, hero3PlayerConfig, ability1, ability2, ability3, colorSet);
    }
    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}

#endif