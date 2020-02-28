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

    [Header("References")]
    [SerializeField] GameSettings gameSettings = null;

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
    [SerializeField] bool preventDoubleLevels = true;
    [SerializeField] float delayAtLevelEnd = 12f;

    [Header("Game Events")]
    [SerializeField] GameEvent levelLoadedEvent = null;

    // Private
    bool manualContinue = false;
    private int matchesPlayed = 0;

    private List<SceneReference> availableLevels = new List<SceneReference>();
    #endregion



    #region Public Properties
    public bool ManualContine { set { manualContinue = value; } }
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
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (SceneReference sr in levels)
        {
            availableLevels.Add(sr);
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
    /// Starts next Level (will count as Played Level)
    /// </summary>
    public void StartNextLevel(float optionalDelay = -1f)
    {
        if (!manualContinue)
        {
            matchesPlayed += 1;
            LoadNextLevel(optionalDelay);
        }
    }

    /// <summary>
    /// Loads next Level of current LevelSet or Credits if Last Level of Set is active
    /// </summary>
    public void LoadNextLevel(float optionalDelay = -1f)
    {
        //bool levelFound = false;
        if (gameSettings.NumberOfMatches != -1 && matchesPlayed > gameSettings.NumberOfMatches)
        {
            StartCoroutine(LoadCreditsCoroutine(optionalDelay >= 0f ? optionalDelay : delayAtLevelEnd));
            return;
        }

        SceneReference nextLevel = GetRandomLevel();

        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (lobby.Equals(activeScene))
        {
            LoadNextScene(nextLevel);
            LoadUIAdditive();
            return;
        }
       
        if (nextLevel.Equals(credits))
        {
            StartCoroutine(LoadCreditsCoroutine(optionalDelay >= 0f ? optionalDelay : delayAtLevelEnd));
            Time.timeScale = 0.0f;
        }
        else
        {
            StartCoroutine(LoadNextLevelCoroutine(nextLevel, optionalDelay >= 0f ? optionalDelay : delayAtLevelEnd));
            Time.timeScale = 0.0f;
        }


        /*
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
                    if (i < levels.Count - 1)
                    {
                        SceneReference nextScene = levels[i + 1];
                        StartCoroutine(LoadNextLevelCoroutine(nextScene, optionalDelay >= 0f ? optionalDelay : delayAtLevelEnd));
                        Time.timeScale = 0.0f;
                        levelFound = true;
                    }
                    else
                    {
                        StartCoroutine(LoadCreditsCoroutine(optionalDelay >= 0f ? optionalDelay : delayAtLevelEnd));
                        Time.timeScale = 0.0f;
                        levelFound = true;
                    }
                }
            }
            if (!levelFound)
            {
                Debug.LogError("There isn't a next Level in this Level Set!");
            }
        }*/
    }

    public void ReloadLevel()
    {
        bool loadUIAdditive = false;
        int uiToLoad = 0;
        // if more then one Scene is open (UI Scene)
        if (UnityEngine.SceneManagement.SceneManager.sceneCount == 2)
        {
            loadUIAdditive = true;
            // Then get that UI Scene's build Index
            uiToLoad = UnityEngine.SceneManagement.SceneManager.GetSceneAt(1).buildIndex;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        // And Reload it Additive after the Active Scene loading is started
        if (loadUIAdditive)
            UnityEngine.SceneManagement.SceneManager.LoadScene(uiToLoad, LoadSceneMode.Additive);
    }

    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenu);
    }

    public void LoadCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(credits);
        LoadUIAdditive(uiCredits);
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
        LoadUIAdditive(uiLobby);
    }

    /// <summary>
    /// Only use this for EditorStartup!
    /// </summary>
    public void LoadUIAdditive(SceneReference sceneToLoad = null)
    {
        if (sceneToLoad != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
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
            if(EditorLevelStarter.Instance != null)
                EditorLevelStarter.Instance.Initialize();
        }
#endif
        if (mode == LoadSceneMode.Additive)
        {
            RaiseLevelLoaded();
        }
    }

    private SceneReference GetRandomLevel()
    {
        if (availableLevels.Count == 0)
        {
            if (preventDoubleLevels || !gameSettings.UseBestOfFeature)
            {
                // All Levels played, break the Game up here
                if(gameSettings.NumberOfMatches != -1)
                    return credits;


                // Repopulate available Levels

                foreach(SceneReference sr in levels)
                {
                    availableLevels.Add(sr);
                }
            }
            else
            {
                // If this Error is thrown there are no Levels set to this SceneManager OR something switched the preventDoubleLevels variable while in Playmode!
                Debug.LogError("There are no available Levels left! Are there Levels linked to the SceneManager?");
                return credits;
            }
        }

        int random = Random.Range(0, availableLevels.Count);
        SceneReference chosenLevel = availableLevels[random];

        if (preventDoubleLevels)
        {
            availableLevels.Remove(chosenLevel);
        }

        return chosenLevel;
    }
    #endregion



    #region GameEvent Raiser
    private void RaiseLevelLoaded()
    {
        levelLoadedEvent.Raise(this);
    }
    #endregion



    #region Coroutines
    IEnumerator LoadNextLevelCoroutine(SceneReference nextScene, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadNextScene(nextScene);
        Time.timeScale = 1;
        LoadUIAdditive();
    }

    IEnumerator LoadCreditsCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadCredits();
        Time.timeScale = 1;
    }
    #endregion
}