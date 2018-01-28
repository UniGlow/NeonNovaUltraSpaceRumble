using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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


    private float passedTime;
    private Boss boss;
    AudioSource audioSource;
    bool colorChangeSoundPlayed;

    public static GameManager Instance;
    #endregion



    #region Unity Event Functions
    private void OnEnable() {
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
    }

    private void Update() {
        passedTime += Time.deltaTime;

        HandleColorSwitch();
	}

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    #endregion



    #region Custom Event Functions
    // Every child of SubscribedBehaviour can implement these
    protected override void OnLevelCompleted() {
        
    }
    #endregion



    #region Public Functions    
    public void NextLevel() {
        StartCoroutine(StartNextLevel());
    }

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
        passedTime = 0f;
        boss = GameObject.FindObjectOfType<Boss>();
        AudioManager.Instance.StartBackgroundTrack();
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
    #endregion



    IEnumerator StartNextLevel() {
        yield return new WaitForSecondsRealtime(delayAtLevelEnd);
        LoadNextScene();
        Time.timeScale = 1;
    }
}
