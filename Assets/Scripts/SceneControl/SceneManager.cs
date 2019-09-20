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
    [SerializeField] List<SceneReference> levels = null;
    [SerializeField] SceneReference mainMenu = null;
    [SerializeField] SceneReference lobby = null;
    [SerializeField] SceneReference credits = null;
    [SerializeField] SceneReference title = null;
    [SerializeField] SceneReference tutorial = null;
    [SerializeField] SceneReference uiScene = null;

    [Header("References")]
    [SerializeField] GameEvent levelLoadedEvent = null;
    [SerializeField] GameEvent levelStartedEvent = null;

    [Header("Properties")]
    [SerializeField] float delayAtLevelEnd = 12f;
    [SerializeField] Points points = null; // Temporal!
    [SerializeField] GameSettings gameSettings = null;
    // Private
    float countdownDuration = 4f;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    protected void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;
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
            Destroy(this);
        }
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLevelFinishedLoading;
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
    #endregion



    #region Private Functions
    void OnLevelFinishedLoading(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        foreach(SceneReference sr in levels)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().path == sr.ScenePath)
            {
                // TODO: Kommentar entfernen, wenn UI Scene einzeln ist / !!!!!!!!!!!!!!!! ACHTUNG! NACHFOLGENDE ZEILE LÄSST UNITY BEI PLAY ABSTÜRZEN!!!!!
                //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(uiScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                if (gameSettings.OverrideLevelPointLimits) GameManager.Instance.OverrideLevelPointLimits();
                StartCoroutine(StartTheAction());
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().path == lobby.ScenePath)
        {
            StartCoroutine(StartTheTutorial());
        }

        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().path == credits.ScenePath)
        {
            StartCoroutine(StartTheCredits());
        }
    }
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

    IEnumerator StartTheAction()
    {
        RaiseLevelLoaded(countdownDuration);
        points.ResetPoints(false);

        yield return new WaitForSecondsRealtime(countdownDuration * (1f / 4f));

        AudioManager.Instance.StartRandomTrack();

        yield return new WaitForSecondsRealtime(countdownDuration * (3f / 4f));

        RaiseLevelStarted();
    }

    IEnumerator StartTheTutorial()
    {
        GameObject.FindObjectOfType<SirAlfredLobby>().Initialize();


        RaiseLevelLoaded(countdownDuration / 4f);

        yield return new WaitForSecondsRealtime(countdownDuration / 4f);

        RaiseLevelStarted();
    }

    IEnumerator StartTheCredits()
    {
        points.ResetPoints(true);

        yield return new WaitForSecondsRealtime(countdownDuration * (3f / 4f));

        RaiseLevelStarted();
    }
    #endregion
}