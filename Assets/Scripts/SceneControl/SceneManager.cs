using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] SceneReference uiLevel = null;
    [SerializeField] SceneReference uiLobby = null;
    [SerializeField] SceneReference uiCredits = null;
    [Space]
    [SerializeField] List<SceneReference> levels = new List<SceneReference>();

    [Header("Properties")]
    [SerializeField] float delayAtLevelEnd = 12f;

    [Header("Game Events")]
    [SerializeField] GameEvent levelLoadedEvent = null;

    // Private
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

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
            Destroy(gameObject);
        }
    }
    #endregion



    #region Public Functions
    /// <summary>
    /// Loads the next scene in build index
    /// </summary>
    public void LoadNextScene(SceneReference nextScene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
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

    /// <summary>
    /// Loads next Level of current LevelSet or Credits if Last Level of Set is active
    /// </summary>
    public void LoadNextLevel()
    {
        bool levelFound = false;
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (lobby.Equals(activeScene))
        {
            LoadNextScene(levels[0]);
            LoadUIAdditive();
        }
        else
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].Equals(activeScene))
                {
                    if (i < levels.Count-1)
                    {
                        SceneReference nextScene = levels[i + 1];
                        StartCoroutine(LoadNextLevelCoroutine(nextScene));
                        Time.timeScale = 0.0f;
                        levelFound = true;
                    }
                    else
                    {
                        StartCoroutine(LoadCreditsCoroutine());
                        Time.timeScale = 0.0f;
                        levelFound = true;
                    }
                }
            }
            if (!levelFound)
            {
                Debug.LogError("There isn't a next Level in this Level Set!");
            }
        }
    }

    public void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        LoadUIAdditive();
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

    /// <summary>
    /// Only use this for EditorStartup!
    /// </summary>
    public void LoadUIAdditive()
    {
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (lobby.Equals(activeScene))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(uiLobby, LoadSceneMode.Additive);
        }else if (credits.Equals(activeScene))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(uiCredits, LoadSceneMode.Additive);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(uiLevel, LoadSceneMode.Additive);
        }
    }
    #endregion



    #region Private Functions
    protected void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
#if UNITY_EDITOR
        if (mode != LoadSceneMode.Additive)
        {
            EditorLevelStarter.Instance.Initialize();
        }
#endif
        if (mode == LoadSceneMode.Additive)
        {
            RaiseLevelLoaded();
        }
    }
    #endregion



    #region GameEvent Raiser
    private void RaiseLevelLoaded()
    {
        levelLoadedEvent.Raise(this);
    }
    #endregion



    #region Coroutines
    IEnumerator LoadNextLevelCoroutine(SceneReference nextScene)
    {
        yield return new WaitForSecondsRealtime(delayAtLevelEnd);
        LoadNextScene(nextScene);
        Time.timeScale = 1;
        LoadUIAdditive();
    }

    IEnumerator LoadCreditsCoroutine()
    {
        yield return new WaitForSecondsRealtime(delayAtLevelEnd);
        LoadCredits();
        Time.timeScale = 1;
    }
    #endregion
}