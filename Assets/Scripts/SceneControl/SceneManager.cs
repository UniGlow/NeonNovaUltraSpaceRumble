using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SceneManager : MonoBehaviour
{

    #region Variable Declarations
    // Serialized Fields
    public static SceneManager Instance;

    [Header("Scenes")]
    [SerializeField] SceneReference mainMenu = null;
    [SerializeField] SceneReference lobby = null;
    [SerializeField] SceneReference credits = null;
    [SerializeField] SceneReference title = null;
    [SerializeField] SceneReference tutorial = null;

    [Header("References")]
    [SerializeField] GameEvent levelLoadedEvent = null;
    [SerializeField] GameEvent levelStartedEvent = null;

    [Header("Properties")]
    [SerializeField] float delayAtLevelEnd = 12f;

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    void Awake()
    {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of an SceneManager.
            Debug.Log("There can only be one SceneManager instantiated. Destroying this Instance...");
            Destroy(this);
        }
    }
    #endregion



    #region Public Functions
    /// <summary>
    /// Loads the next scene in build index
    /// </summary>
    public void LoadNextScene()
    {
        int activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (activeScene + 1 < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(activeScene + 1);
        }
        else
        {
            Debug.LogError("No more levels in build index to be loaded");
        }
    }

    public void LoadLevel(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }

    /// <summary>
    /// Quits the application or exits play mode when in editor
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Exiting the game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public string GetActiveSceneName()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelCoroutine());
        Time.timeScale = 0.0f;
    }

    public void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenu);
    }

    public void LoadCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(credits);
    }

    public void LoadTitleScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(title);
    }

    public void LoadTutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(tutorial);
    }

    public void LoadLobby()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(lobby);
    }
    #endregion



    #region Private Functions

    #endregion



    #region GameEvent Raiser

    void RaiseLevelStarted()
    {
        levelStartedEvent.Raise(this);
    }

    void RaiseLevelLoaded(float duration)
    {
        levelLoadedEvent.Raise(this, duration);
    }
    #endregion



    #region Coroutines
    IEnumerator LoadNextLevelCoroutine()
    {
        yield return new WaitForSecondsRealtime(delayAtLevelEnd);
        LoadNextScene();
        Time.timeScale = 1;
    }
    #endregion
}