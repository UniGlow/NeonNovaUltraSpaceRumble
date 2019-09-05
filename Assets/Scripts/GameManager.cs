using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

/// <summary>
/// Manages the overall flow of the game and scene loading. This class is a singleton and won't be destroyed when loading a new scene.
/// </summary>
public class GameManager : MonoBehaviour
{

    #region Variable Declarations
    public static GameManager Instance;

    [Header("Game Properties")]
    [SerializeField] float critDamageMultiplier = 2f;
    public float CritDamageMultiplier { get { return critDamageMultiplier; } set { critDamageMultiplier = value; } }
    public float intensifyTime = 60;
    [Range(0f, 0.9f)]
    public float intensifyAmount = 0.3f;
    [SerializeField] float delayAtLevelEnd = 12f;
    [Space]
    [SerializeField] bool overrideLevelPointLimits;
    public int heroesWinningPointLead = 500;
    public int bossWinningPointLead = 500;

    [Header("AI Adjustments")]
    public int bossWinningSolo = 500;
    public int bossWinningDuo = 500;
    public int bossWinningTriple = 500;

    [Header("References")]
    [SerializeField] GameEvent levelStartedEvent = null;
    [SerializeField] GameEvent levelLoadedEvent = null;

    // TODO: Verlagern in SO "GameSettings"
    [HideInInspector] public ColorSet activeColorSet = null;



    private Boss boss;
    public Boss Boss { get { return boss; } }
    float intensifyTimer;
    readonly float countdownDuration = 4f;
    #endregion



    #region Unity Event Functions
    protected void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    //Awake is always called before any Start functions
    void Awake() {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this) {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of an AudioManager.
            Debug.Log("There can only be one GameManager instantiated. Destroying this Instance...");
            Destroy(this);
        }
    }

    private void Start() {
        boss = GameObject.FindObjectOfType<Boss>();

#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }

    private void Update()
    {
        intensifyTimer += Time.deltaTime;

        HandleIntensify();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    #endregion



    #region Public Functions
    /// <summary>
    /// Loads the next scene in build index
    /// </summary>
    public void LoadNextScene()
    {
        int activeScene = SceneManager.GetActiveScene().buildIndex;
        if (activeScene + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(activeScene + 1);
        }
        else
        {
            Debug.LogError("No more levels in build index to be loaded");
        }
    }

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
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
        return SceneManager.GetActiveScene().name;
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelCoroutine());
        Time.timeScale = 0.0f;
    }

    public void ResetTimer()
    {
        intensifyTimer = 0f;
    }
    #endregion



    #region Private Functions
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        boss = GameObject.FindObjectOfType<Boss>();

        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            if (SceneManager.GetActiveScene().name.Contains("Lobby"))
            {
                StartCoroutine(StartTheTutorial());
            }
            else
            {
                if (overrideLevelPointLimits) OverrideLevelPointLimits();
                StartCoroutine(StartTheAction());
            }
        }

        else if (SceneManager.GetActiveScene().name.Contains("Credit"))
        {
            GameObject.FindGameObjectWithTag(Constants.TAG_HOMING_MISSILE).GetComponent<HomingMissile>().PauseMissile(true);
            StartCoroutine(StartTheCredits());
        }
    }

    void HandleIntensify()
    {
        if (intensifyTimer >= intensifyTime && BossHealth.Instance && HeroHealth.Instance)
        {
            // Set new winningPointLeads
            BossHealth.Instance.WinningPointLead = Mathf.RoundToInt(BossHealth.Instance.WinningPointLead * (1 - intensifyAmount));
            HeroHealth.Instance.WinningPointLead = Mathf.RoundToInt(HeroHealth.Instance.WinningPointLead * (1 - intensifyAmount));

            // Close the damage gap to keep the healthbar seemingly unchanged
            HeroHealth.Instance.CurrentDamage = Mathf.RoundToInt(HeroHealth.Instance.CurrentDamage * (1 - intensifyAmount));
            BossHealth.Instance.CurrentDamage = Mathf.RoundToInt(BossHealth.Instance.CurrentDamage * (1 - intensifyAmount));

            intensifyTimer = 0f;
        }
    }

    void OverrideLevelPointLimits()
    {
        HeroHealth.Instance.WinningPointLead = heroesWinningPointLead;
        BossHealth.Instance.WinningPointLead = bossWinningPointLead;
    }

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
        AudioManager.Instance.StartRandomTrack();

        yield return new WaitForSecondsRealtime(countdownDuration);

        RaiseLevelStarted();
    }

    IEnumerator StartTheTutorial()
    {
        //TODO: Alfred starten
        GameObject.FindObjectOfType<SirAlfredLobby>().Initialize();


        RaiseLevelLoaded(countdownDuration / 4f);

        yield return new WaitForSecondsRealtime(countdownDuration / 4f);

        RaiseLevelStarted();
    }

    IEnumerator StartTheCredits()
    {

        yield return new WaitForSecondsRealtime(countdownDuration / 4f);

        RaiseLevelStarted();
    }
    #endregion
}
