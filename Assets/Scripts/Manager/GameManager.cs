using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages the overall flow of the game and scene loading. This class is a singleton and won't be destroyed when loading a new scene.
/// </summary>
public class GameManager : SubscribedBehaviour {

    #region Variable Declarations
    [Header("Game Properties")]
    [SerializeField] float colorSwitchInterval = 10f;
    [SerializeField] float critDamageMultiplier = 2f;
    public float CritDamageMultiplier { get { return critDamageMultiplier; } }
    [SerializeField] float delayAtLevelEnd = 12f;
    float delayForActionStart = 4f;

    [Header("Sound")]
    [SerializeField] AudioClip colorChangeSound;
    [Range(0, 1)]
    [SerializeField] float colorChangeSoundVolume = 1f;

    [Header("References")]
    [SerializeField] Color greenPlayerColor;
    public Color GreenPlayerColor { get { return greenPlayerColor; } }
    [SerializeField] Color redPlayerColor;
    public Color RedPlayerColor { get { return redPlayerColor; } }
    [SerializeField] Color bluePlayerColor;
    public Color BluePlayerColor { get { return bluePlayerColor; } }
    [SerializeField] GameObject heroAIPrefab;
    [SerializeField] GameObject bossAIPrefab;



    private float passedTime;
    private Boss boss;
    public Boss Boss { get { return boss; } }
    AudioSource audioSource;
    bool colorChangeSoundPlayed;
    TextMeshProUGUI winText;
    int playerCount;
    public int PlayerCount { get { return playerCount; } }

    public static GameManager Instance;
    #endregion



    #region Unity Event Functions
    override protected void OnEnable()
    {
        base.OnEnable();

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
        audioSource = GetComponent<AudioSource>();

        playerCount = Input.GetJoystickNames().Length;

#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }

    private void Update() {
        passedTime += Time.deltaTime;

        HandleColorSwitch();
	}

    override protected void OnDisable()
    {
        base.OnDisable();

        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    #endregion



    #region Custom Event Functions
    // Every child of SubscribedBehaviour can implement these
    override protected void OnLevelCompleted(string winner) {
        StartCoroutine(LoadNextLevel());
        Time.timeScale = 0.0f;
    }

    protected override void OnLevelStarted()
    {
        passedTime = 0f;
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
    #endregion



    #region Private Functions
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        boss = GameObject.FindObjectOfType<Boss>();

        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {

            winText = GameObject.FindObjectOfType<HeroHealth>().WinText;
            GameObject.FindGameObjectWithTag(Constants.TAG_HOMING_MISSILE).GetComponent<HomingMissile>().PauseMissile(true);

            if (SceneManager.GetActiveScene().name.Contains("Tutorial"))
            {
                StartCoroutine(StartTheTutorial());
            }
            else
            {
                SetupAICharacters();
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
            if (passedTime >= colorSwitchInterval - 0.5f && !colorChangeSoundPlayed) {
                audioSource.PlayOneShot(colorChangeSound, colorChangeSoundVolume);
                colorChangeSoundPlayed = true;
            }
            // Set new Boss color
            if (passedTime >= colorSwitchInterval) {
                ChangeBossColor();
                passedTime = 0f;
                colorChangeSoundPlayed = false;
            }
        }
    }

    /// <summary>
    /// Randomly changes one of the boss colors (weakness or strength)
    /// </summary>
    void ChangeBossColor() {
        // Set strength color
        if (Random.Range(0, 2) == 0) {
            if (boss.StrengthColor == PlayerColor.Blue) {
                if (Random.Range(0, 2) == 0) {
                    boss.SetStrengthColor(PlayerColor.Green);
                }
                else {
                    boss.SetStrengthColor(PlayerColor.Red);
                }
            } else if (boss.StrengthColor == PlayerColor.Green) {
                if (Random.Range(0, 2) == 0) {
                    boss.SetStrengthColor(PlayerColor.Blue);
                }
                else {
                    boss.SetStrengthColor(PlayerColor.Red);
                }
            } else if (boss.StrengthColor == PlayerColor.Red) {
                if (Random.Range(0, 2) == 0) {
                    boss.SetStrengthColor(PlayerColor.Green);
                }
                else {
                    boss.SetStrengthColor(PlayerColor.Blue);
                }
            }
        }
        // Or set weakness color
        else {
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
        }
    }

    void SetupAICharacters()
    {
        playerCount = Input.GetJoystickNames().Length;

        // Get references
        GameObject boss = GameObject.FindGameObjectWithTag(Constants.TAG_BOSS);

        GameObject[] heroes = GameObject.FindGameObjectsWithTag(Constants.TAG_HERO);
        GameObject damage = new GameObject();
        GameObject tank = new GameObject();
        GameObject opfer = new GameObject();
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

            // Set AI HealthIndicators
            GameObject levelScrpits = GameObject.Find("_LEVEL_SCRIPTS");
            levelScrpits.GetComponent<HeroHealth>().healthIndicators[2] = damageAI.healthIndicator;
            levelScrpits.GetComponent<HeroHealth>().healthIndicators[1] = tankAI.healthIndicator;
            levelScrpits.GetComponent<HeroHealth>().healthIndicators[0] = opferAI.healthIndicator;

            // Set camera targets
            MultipleTargetCamera cameraRig = Camera.main.transform.parent.GetComponent<MultipleTargetCamera>();
            cameraRig.targets[2] = damageAI.transform;
            cameraRig.targets[1] = tankAI.transform;
            cameraRig.targets[0] = opferAI.transform;

            // Set boss playerNumber and health
            boss.transform.parent.GetComponent<Player>().PlayerNumber = 1;
            levelScrpits.GetComponent<BossHealth>().MaxHealth = 1500;
        }
        else if(playerCount == 2)
        {
            // Replace damage hero with AI
            HeroAI opferAI = GameObject.Instantiate(heroAIPrefab, opfer.transform.position, opfer.transform.rotation).GetComponent<HeroAI>();
            opferAI.ability = Ability.Opfer;
            opferAI.PlayerColor = opfer.transform.parent.GetComponent<Hero>().PlayerColor;
            Destroy(opfer.transform.parent.gameObject);


            // Replace Boss with AI
            BossAI bossAI = GameObject.Instantiate(bossAIPrefab, boss.transform.position, boss.transform.rotation).GetComponent<BossAI>();
            bossAI.SetStrengthColor(boss.transform.parent.GetComponent<Boss>().StrengthColor);
            bossAI.SetWeaknessColor(boss.transform.parent.GetComponent<Boss>().WeaknessColor);
            Destroy(boss.transform.parent.gameObject);

            // Set AI HealthIndicators
            GameObject levelScrpits = GameObject.Find("_LEVEL_SCRIPTS");
            levelScrpits.GetComponent<HeroHealth>().healthIndicators[0] = opferAI.healthIndicator;
            levelScrpits.GetComponent<BossHealth>().HealthIndicator = bossAI.healthIndicator;

            // Set camera targets
            MultipleTargetCamera cameraRig = Camera.main.transform.parent.GetComponent<MultipleTargetCamera>();
            cameraRig.targets[0] = opferAI.transform;
            cameraRig.targets[3] = bossAI.transform;

            // Set tank and opfer playerNumber and health
            damage.transform.parent.GetComponent<Player>().PlayerNumber = 1;
            tank.transform.parent.GetComponent<Player>().PlayerNumber = 2;
            levelScrpits.GetComponent<BossHealth>().MaxHealth = 1000;
        }
        else if (playerCount == 3)
        {
            // Replace Boss with AI
            BossAI bossAI = GameObject.Instantiate(bossAIPrefab, boss.transform.position, boss.transform.rotation).GetComponent<BossAI>();
            bossAI.SetStrengthColor(boss.transform.parent.GetComponent<Boss>().StrengthColor);
            bossAI.SetWeaknessColor(boss.transform.parent.GetComponent<Boss>().WeaknessColor);
            Destroy(boss.transform.parent.gameObject);

            // Set AI HealthIndicators
            GameObject levelScrpits = GameObject.Find("_LEVEL_SCRIPTS");
            levelScrpits.GetComponent<BossHealth>().HealthIndicator = bossAI.healthIndicator;

            // Set camera targets
            MultipleTargetCamera cameraRig = Camera.main.transform.parent.GetComponent<MultipleTargetCamera>();
            cameraRig.targets[3] = bossAI.transform;

            // Set hero playerNumbers and health
            damage.transform.parent.GetComponent<Player>().PlayerNumber = 1;
            tank.transform.parent.GetComponent<Player>().PlayerNumber = 2;
            opfer.transform.parent.GetComponent<Player>().PlayerNumber = 3;
            levelScrpits.GetComponent<BossHealth>().MaxHealth = 1000;
        }
        else {
            // Do nothing for a 4 player game (scene is already set up for this)
        }
    }
    #endregion



    IEnumerator LoadNextLevel() {
        yield return new WaitForSecondsRealtime(delayAtLevelEnd);
        LoadNextScene();
        Time.timeScale = 1;
    }

    IEnumerator StartTheAction() {

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);

        winText.gameObject.SetActive(true);
        AudioManager.Instance.StartRandomTrack();

        Vector3 originalScale = winText.transform.localScale;
        winText.transform.localScale = Vector3.zero;
        winText.text = "3";
        LeanTween.scale(winText.gameObject, originalScale, 0.7f).setEase(LeanTweenType.easeOutBounce).setIgnoreTimeScale(true);

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);
        
        winText.transform.localScale = Vector3.zero;
        winText.text = "2";
        LeanTween.scale(winText.gameObject, originalScale, 0.7f).setEase(LeanTweenType.easeOutBounce).setIgnoreTimeScale(true);

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);
        
        winText.transform.localScale = Vector3.zero;
        winText.text = "1";
        LeanTween.scale(winText.gameObject, originalScale, 0.7f).setEase(LeanTweenType.easeOutBounce).setIgnoreTimeScale(true);

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);

        winText.gameObject.SetActive(false);
        GameEvents.StartLevelStarted();
    }

    IEnumerator StartTheTutorial()
    {

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);
        
        AudioManager.Instance.StartTutorialTrack();
        
        GameEvents.StartLevelStarted();
    }

    IEnumerator StartTheCredits()
    {

        yield return new WaitForSecondsRealtime(delayForActionStart / 4f);

        AudioManager.Instance.StartRandomTrack();

        GameEvents.StartLevelStarted();
    }
}
