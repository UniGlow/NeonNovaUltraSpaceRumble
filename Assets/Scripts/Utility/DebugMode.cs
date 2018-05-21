using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides a Debug Mode Menu
/// </summary>
public class DebugMode : MonoBehaviour
{

    public static DebugMode Instance;

    [HideInInspector] public string colorSwitchInterval;
    [HideInInspector] public string critDamageMultiplier;
    [HideInInspector] public string intensifyTime;
    [HideInInspector] public string intensifyAmount;
    [HideInInspector] public string heroesWinningPointLead;
    [HideInInspector] public string bossWinningPointLead;

    private bool debugMode = false;



    //Awake is always called before any Start functions
    private void Awake() {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this) { 
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a DebugMode.
            Debug.Log("There can only be one DebugMode instantiated. Destroying this Instance...");
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        colorSwitchInterval = GameManager.Instance.ColorSwitchInterval.ToString();
        critDamageMultiplier = GameManager.Instance.CritDamageMultiplier.ToString();
        intensifyTime = GameManager.Instance.intensifyTime.ToString();
        intensifyAmount = GameManager.Instance.intensifyAmount.ToString();
        heroesWinningPointLead = GameManager.Instance.heroesWinningPointLead.ToString();
        bossWinningPointLead = GameManager.Instance.bossWinningPointLead.ToString();
    }

    private void Update () {
        // Switch the Games Debug Mode On/Off
        if (Input.GetButtonDown(Constants.INPUT_DEBUGMODE)) {
            debugMode = !debugMode;
        }
    }



    #region Custom Event Functions
    // Every child of SubscribedBehaviour can implement these
    #endregion
    


    // Draws the GUI for the Debug Mode and declares it's functionality
    private void OnGUI() {
        if (debugMode) {
            // Setup of the box and title
            GUILayout.BeginVertical("box", GUILayout.Width(Screen.width * 0.2f));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Debug Menu");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Button for loading the next scene by buildIndex
            GUILayout.BeginVertical("box");
            GUILayout.Label("Scene Management:");
            if (GUILayout.Button("Load next scene")) {
                Debug.Log("Debug Mode: Loading next scene.");
                GameManager.Instance.LoadNextScene();
            }
            if (GUILayout.Button("Reload scene")) {
                Debug.Log("Debug Mode: Reloading current scene.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            GUILayout.EndVertical();

            // Balancing Parameters
            int newColorSwitchInterval;
            double newCritDamageMultiplier;
            int newIntensifyTime;
            double newIntensifyAmount;
            int newHeroesPoints;
            int newBossPoints;

            GUILayout.BeginVertical("box");
            GUILayout.Label("Balancing Parameters");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Color Switch Interval");
            colorSwitchInterval = GUILayout.TextField(colorSwitchInterval);
            if (System.Int32.TryParse(colorSwitchInterval, out newColorSwitchInterval)) GameManager.Instance.ColorSwitchInterval = newColorSwitchInterval;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Crit Damage Multiplier");
            critDamageMultiplier = GUILayout.TextField(critDamageMultiplier);
            if (System.Double.TryParse(critDamageMultiplier, out newCritDamageMultiplier)) GameManager.Instance.CritDamageMultiplier = (float) newCritDamageMultiplier;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intensify Time");
            intensifyTime = GUILayout.TextField(intensifyTime);
            if (System.Int32.TryParse(intensifyTime, out newIntensifyTime)) GameManager.Instance.intensifyTime = newIntensifyTime;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intensify Amount");
            intensifyAmount = GUILayout.TextField(intensifyAmount);
            if (System.Double.TryParse(intensifyAmount, out newIntensifyAmount)) GameManager.Instance.intensifyAmount = (float) newIntensifyAmount;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Heroes Winning Point Lead");
            heroesWinningPointLead = GUILayout.TextField(heroesWinningPointLead);
            if (System.Int32.TryParse(heroesWinningPointLead, out newHeroesPoints)) GameManager.Instance.heroesWinningPointLead = newHeroesPoints;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Boss Winning Point Lead");
            bossWinningPointLead = GUILayout.TextField(bossWinningPointLead);
            if (System.Int32.TryParse(bossWinningPointLead, out newBossPoints)) GameManager.Instance.bossWinningPointLead = newBossPoints;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
