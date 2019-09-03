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
    [Header("Game Properties")]
    [SerializeField] float colorSwitchInterval = 10f;
    public float ColorSwitchInterval { get { return colorSwitchInterval; } set { colorSwitchInterval = value; } }
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
    [Space]
    [SerializeField] float countdownDuration = 4f;

    [Header("Sound")]
    [SerializeField] AudioClip colorChangeSound;
    [Range(0, 1)]
    [SerializeField] float colorChangeSoundVolume = 1f;
    [SerializeField] AudioSource colorChangeAudioSource = null;

    [Header("AI Adjustments")]
    [SerializeField] bool setupAI = true;
    public int bossWinningSolo = 500;
    public int bossWinningDuo = 500;
    public int bossWinningTriple = 500;

    [Header("References")]
    [SerializeField] Color greenPlayerColor;
    public Color GreenPlayerColor { get { return greenPlayerColor; } }
    [SerializeField] Color redPlayerColor;
    public Color RedPlayerColor { get { return redPlayerColor; } }
    [SerializeField] Color bluePlayerColor;
    public Color BluePlayerColor { get { return bluePlayerColor; } }
    [SerializeField] GameObject heroAIPrefab;
    [SerializeField] GameObject bossAIPrefab;
    [SerializeField] GameEvent levelStartedEvent = null;
    [SerializeField] GameEvent countdownStartedEvent = null;



    private float colorChangeTimer;
    float delayForActionStart = 4f;
    private Boss boss;
    public Boss Boss { get { return boss; } }
    bool colorChangeSoundPlayed;
    int playerCount;
    public int PlayerCount { get { return playerCount; } }
    float intensifyTimer;

    public static GameManager Instance;
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

        UpdatePlayerCount();

#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }

    private void Update()
    {
        colorChangeTimer += Time.deltaTime;
        intensifyTimer += Time.deltaTime;

        HandleColorSwitch();

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
    public void LoadNextScene() {
        int activeScene = SceneManager.GetActiveScene().buildIndex;
        if (activeScene + 1 < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(activeScene + 1);
        }
        else {
            Debug.LogError("No more levels in build index to be loaded");
        }
    }

    public void LoadLevel(string name) {
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// Quits the application or exits play mode when in editor
    /// </summary>
    public void ExitGame() {
        Debug.Log("Exiting the game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ResetPassedTimeForColorChange()
    {
        colorChangeTimer = 0f;
    }

    public string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void UpdatePlayerCount()
    {
        playerCount = 0;
        string[] joystickNames = Input.GetJoystickNames();
        foreach (string name in joystickNames)
        {
            if (name != "")
            {
                playerCount++;
            }
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelCoroutine());
        Time.timeScale = 0.0f;
    }

    public void ResetTimer()
    {
        colorChangeTimer = 0f;
        intensifyTimer = 0f;
    }
    #endregion



    #region Private Functions
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        boss = GameObject.FindObjectOfType<Boss>();

        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            
            GameObject.FindGameObjectWithTag(Constants.TAG_HOMING_MISSILE).GetComponent<HomingMissile>().PauseMissile(true);

            if (SceneManager.GetActiveScene().name.Contains("Tutorial"))
            {
                StartCoroutine(StartTheTutorial());
            }
            else
            {
                if (overrideLevelPointLimits) OverrideLevelPointLimits();
                if (setupAI) SetupAICharacters();
                StartCoroutine(StartTheAction());
            }
        }

        else if (SceneManager.GetActiveScene().name.Contains("Credit"))
        {
            GameObject.FindGameObjectWithTag(Constants.TAG_HOMING_MISSILE).GetComponent<HomingMissile>().PauseMissile(true);
            StartCoroutine(StartTheCredits());
        }
    }

    void HandleColorSwitch() {
        if (SceneManager.GetActiveScene().name.Contains("Level")) {
            if (colorChangeTimer >= colorSwitchInterval - 0.5f && !colorChangeSoundPlayed) {
                colorChangeAudioSource.PlayOneShot(colorChangeSound, colorChangeSoundVolume);
                colorChangeSoundPlayed = true;
            }
            // Set new Boss color
            if (colorChangeTimer >= colorSwitchInterval) {
                ChangeBossColor();
                if (SceneManager.GetActiveScene().name.Contains("Tutorial")) TutorialTextUpdater.BossColorChange();
                colorChangeTimer = 0f;
                colorChangeSoundPlayed = false;
            }
        }
    }

    /// <summary>
    /// Randomly changes one of the boss colors (weakness or strength)
    /// </summary>
    void ChangeBossColor() {
        // Set strength color
        //if (Random.Range(0, 2) == 0) {
        //    if (boss.StrengthColor == PlayerColor.Blue) {
        //        if (Random.Range(0, 2) == 0) {
        //            boss.SetStrengthColor(PlayerColor.Green);
        //        }
        //        else {
        //            boss.SetStrengthColor(PlayerColor.Red);
        //        }
        //    } else if (boss.StrengthColor == PlayerColor.Green) {
        //        if (Random.Range(0, 2) == 0) {
        //            boss.SetStrengthColor(PlayerColor.Blue);
        //        }
        //        else {
        //            boss.SetStrengthColor(PlayerColor.Red);
        //        }
        //    } else if (boss.StrengthColor == PlayerColor.Red) {
        //        if (Random.Range(0, 2) == 0) {
        //            boss.SetStrengthColor(PlayerColor.Green);
        //        }
        //        else {
        //            boss.SetStrengthColor(PlayerColor.Blue);
        //        }
        //    }
        //}
        //// Or set weakness color
        //else {
            if (boss.WeaknessColor == PlayerColor.Blue) {
                if (Random.Range(0, 2) == 0) {
                    boss.SetWeaknessColor(PlayerColor.Green);
                }
                else {
                    boss.SetWeaknessColor(PlayerColor.Red);
                }
            }
            else if (boss.WeaknessColor == PlayerColor.Green) {
                if (Random.Range(0, 2) == 0) {
                    boss.SetWeaknessColor(PlayerColor.Blue);
                }
                else {
                    boss.SetWeaknessColor(PlayerColor.Red);
                }
            }
            else if (boss.WeaknessColor == PlayerColor.Red) {
                if (Random.Range(0, 2) == 0) {
                    boss.SetWeaknessColor(PlayerColor.Green);
                }
                else {
                    boss.SetWeaknessColor(PlayerColor.Blue);
                }
            }
        //}
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

    void SetupAICharacters()
    {
        // Update player count
        UpdatePlayerCount();

        // Get references
        GameObject[] heroes = GameObject.FindGameObjectsWithTag(Constants.TAG_HERO);
        GameObject damage = null;
        GameObject tank = null;
        GameObject opfer = null;
        foreach (GameObject go in heroes)
        {
            if (go.transform.parent.GetComponent<Hero>() == null) continue;
            if (go.transform.parent.GetComponent<Hero>().ability == Ability.Damage) damage = go;
            if (go.transform.parent.GetComponent<Hero>().ability == Ability.Tank) tank = go;
            if (go.transform.parent.GetComponent<Hero>().ability == Ability.Opfer) opfer = go;
        }

        if (playerCount == 1 || playerCount == 0)
        {

            // Replace Heroes with AIs
            HeroAI damageAI = GameObject.Instantiate(heroAIPrefab, damage.transform.position, damage.transform.rotation).GetComponent<HeroAI>();
            damageAI.ability = Ability.Damage;
            damageAI.PlayerColor = damage.transform.parent.GetComponent<Hero>().PlayerColor;
            Destroy(damage.transform.parent.gameObject);

            HeroAI tankAI = GameObject.Instantiate(heroAIPrefab, tank.transform.position, tank.transform.rotation).GetComponent<HeroAI>();
            tankAI.ability = Ability.Tank;
            tankAI.PlayerColor = tank.transform.parent.GetComponent<Hero>().PlayerColor;
            Destroy(tank.transform.parent.gameObject);

            HeroAI opferAI = GameObject.Instantiate(heroAIPrefab, opfer.transform.position, opfer.transform.rotation).GetComponent<HeroAI>();
            opferAI.ability = Ability.Opfer;
            opferAI.PlayerColor = opfer.transform.parent.GetComponent<Hero>().PlayerColor;
            Destroy(opfer.transform.parent.gameObject);

            // Set camera targets
            MultipleTargetCamera cameraRig = Camera.main.transform.parent.GetComponent<MultipleTargetCamera>();
            cameraRig.SetCameraTargetsNextFrame(new List<Transform>()
            {
                damageAI.transform,
                tankAI.transform,
                opferAI.transform
            });

            // Set boss playerNumber and health
            boss.PlayerNumber = 1;
            BossHealth.Instance.WinningPointLead = bossWinningSolo;
        }
        else if (playerCount == 2)
        {
            // Replace damage hero with AI
            HeroAI opferAI = GameObject.Instantiate(heroAIPrefab, opfer.transform.position, opfer.transform.rotation).GetComponent<HeroAI>();
            opferAI.ability = Ability.Opfer;
            opferAI.PlayerColor = opfer.transform.parent.GetComponent<Hero>().PlayerColor;
            Destroy(opfer.transform.parent.gameObject);


            // Replace Boss with AI
            BossAI bossAI = GameObject.Instantiate(bossAIPrefab, boss.transform.position, boss.transform.rotation).GetComponent<BossAI>();
            bossAI.SetStrengthColor(boss.StrengthColor);
            bossAI.SetWeaknessColor(boss.WeaknessColor);
            Destroy(boss.gameObject);
            boss = bossAI;


            // Set tank and opfer playerNumber and health
            damage.transform.parent.GetComponent<Player>().PlayerNumber = 1;
            tank.transform.parent.GetComponent<Player>().PlayerNumber = 2;
            BossHealth.Instance.WinningPointLead = bossWinningDuo;

            // Set camera targets
            MultipleTargetCamera cameraRig = Camera.main.transform.parent.GetComponent<MultipleTargetCamera>();
            cameraRig.SetCameraTargetsNextFrame(new List<Transform>()
            {
                opferAI.transform,
                bossAI.transform
            });

            // Link references in bossAI
            bossAI.SetHeroReferencesNextFrame();
        }
        else if (playerCount == 3)
        {
            // Replace Boss with AI
            BossAI bossAI = GameObject.Instantiate(bossAIPrefab, boss.transform.position, boss.transform.rotation).GetComponent<BossAI>();
            bossAI.SetStrengthColor(boss.StrengthColor);
            bossAI.SetWeaknessColor(boss.WeaknessColor);
            Destroy(boss.gameObject);
            boss = bossAI;

            // Set camera targets
            MultipleTargetCamera cameraRig = Camera.main.transform.parent.GetComponent<MultipleTargetCamera>();
            cameraRig.SetCameraTargetsNextFrame(new List<Transform>()
            {
                bossAI.transform
            });

            // Set hero playerNumbers and health
            damage.transform.parent.GetComponent<Player>().PlayerNumber = 1;
            tank.transform.parent.GetComponent<Player>().PlayerNumber = 2;
            opfer.transform.parent.GetComponent<Player>().PlayerNumber = 3;
            BossHealth.Instance.WinningPointLead = bossWinningTriple;

            // Link references in bossAI
            bossAI.SetHeroReferencesNextFrame();
        }
        else {
            // Do nothing for a 4 player game (scene is already set up for this)
        }
    }

    void RaiseLevelStarted()
    {
        levelStartedEvent.Raise(this);
    }

    void RaiseCountdownStarted(float duration)
    {
        countdownStartedEvent.Raise(this, duration);
    }
    #endregion



    #region Coroutines
    IEnumerator LoadNextLevelCoroutine() {
        yield return new WaitForSecondsRealtime(delayAtLevelEnd);
        LoadNextScene();
        Time.timeScale = 1;
    }

    IEnumerator StartTheAction() {

        yield return new WaitForSecondsRealtime(countdownDuration / 4f);
        RaiseCountdownStarted(countdownDuration/4f*3f);
        AudioManager.Instance.StartRandomTrack();
        /*
        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);

        winText.gameObject.SetActive(true);

        Vector3 originalScale = winText.transform.localScale;
        winText.transform.localScale = Vector3.zero;
        winText.text = "3";
        winText.transform.DOScale(originalScale, .7f).SetEase(Ease.OutBounce).SetUpdate(true);

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);
        
        winText.transform.localScale = Vector3.zero;
        winText.text = "2";
        winText.transform.DOScale(originalScale, .7f).SetEase(Ease.OutBounce).SetUpdate(true);

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);
        
        winText.transform.localScale = Vector3.zero;
        winText.text = "1";
        winText.transform.DOScale(originalScale, .7f).SetEase(Ease.OutBounce).SetUpdate(true);

        

        winText.gameObject.SetActive(false);*/
        yield return new WaitForSecondsRealtime(countdownDuration);
        RaiseLevelStarted();
    }

    IEnumerator StartTheTutorial()
    {
        UpdatePlayerCount();

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);

        RaiseLevelStarted();
    }

    IEnumerator StartTheCredits()
    {

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);

        RaiseLevelStarted();
    }
    #endregion
}
