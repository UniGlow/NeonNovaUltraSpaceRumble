using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a Debug Mode Menu
/// </summary>
public class DebugMode : MonoBehaviour
{

    public static DebugMode Instance;

    [SerializeField] public GameSettings gameSettings;
    [SerializeField] public Points points;

    [HideInInspector] public string colorSwitchInterval;
    [HideInInspector] public string critDamageMultiplier;
    [HideInInspector] public string intensifyTime;
    [HideInInspector] public string intensifyAmount;
    [HideInInspector] public string pointLeadToWin;
    [HideInInspector] public string optimalScorePerSecond;
    [HideInInspector] public string optimalCritDamage;
    [HideInInspector] public string optimalDamage;
    [HideInInspector] public string optimalShieldingPercentage;
    [HideInInspector] public string optimalBossOrbHits;
    [HideInInspector] public string worstHeroOrbHits;

    private bool debugMode = false;



    //Awake is always called before any Start functions
    private void Awake()
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
        critDamageMultiplier = gameSettings.CritDamageMultiplier.ToString();
        intensifyTime = gameSettings.IntensifyTime.ToString();
        intensifyAmount = gameSettings.IntensifyAmount.ToString();
        pointLeadToWin = points.PointLeadToWin.ToString();
        colorSwitchInterval = gameSettings.BossColorSwitchInterval.ToString();

        // Scoring System
        optimalScorePerSecond = gameSettings.OptimalScorePerSecond.ToString();
        optimalCritDamage = gameSettings.DamageScoreCategories.Find(x => x.name == "CritDamageDone").optimalValue.ToString();
        optimalDamage = gameSettings.DamageScoreCategories.Find(x => x.name == "DamageDone").optimalValue.ToString();
        optimalShieldingPercentage = gameSettings.TankScoreCategories.Find(x => x.name == "DamageShielded").optimalValue.ToString();
        optimalBossOrbHits = gameSettings.RunnerScoreCategories.Find(x => x.name == "OrbBossHits").optimalValue.ToString();
        worstHeroOrbHits = gameSettings.RunnerScoreCategories.Find(x => x.name == "OrbHeroHits").worstValue.ToString();
    }

    private void Update ()
    {
        // Switch the Games Debug Mode On/Off
        if (InputHelper.GetButtonDown(RewiredConsts.Action.DEBUG))
        {
            debugMode = !debugMode;
            if (debugMode)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }



    #region Custom Event Functions
    // Every child of SubscribedBehaviour can implement these
    #endregion
    


    // Draws the GUI for the Debug Mode and declares it's functionality
    private void OnGUI()
    {
        if (debugMode)
        {
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
            if (GUILayout.Button("Load next scene"))
            {
                Debug.Log("Debug Mode: Loading next scene.");
                //TODO: Schnelles Szenenladen ermöglichen, ohne das Delay nach Levelende
                SceneManager.Instance.StartNextLevel();
            }
            if (GUILayout.Button("Reload scene"))
            {
                Debug.Log("Debug Mode: Reloading current scene.");
                SceneManager.Instance.ReloadLevel();
            }
            GUILayout.EndVertical();

            // Balancing Parameters
            double newCritDamageMultiplier;
            int newIntensifyTime;
            double newIntensifyAmount;
            int newHeroesPoints;
            float newColorSwitchInterval;

            // Score Parameters
            int newOptimalScorePerSecond;
            int newOptimalCritDamage;
            int newOptimalDamage;
            int newOptimalShieldingPercentage;
            int newOptimalBossOrbHits;
            int newWorstHeroOrbHits;

            GUILayout.BeginVertical("box");
            GUILayout.Label("Balancing Parameters");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Color Switch Interval");
            colorSwitchInterval = GUILayout.TextField(colorSwitchInterval);
            if (System.Single.TryParse(colorSwitchInterval, out newColorSwitchInterval)) gameSettings.BossColorSwitchInterval = newColorSwitchInterval;
            GUILayout.EndHorizontal();
            

            GUILayout.BeginHorizontal();
            GUILayout.Label("Crit Damage Multiplier");
            critDamageMultiplier = GUILayout.TextField(critDamageMultiplier);
            if (System.Double.TryParse(critDamageMultiplier, out newCritDamageMultiplier)) gameSettings.CritDamageMultiplier = (float) newCritDamageMultiplier;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intensify Time");
            intensifyTime = GUILayout.TextField(intensifyTime);
            if (System.Int32.TryParse(intensifyTime, out newIntensifyTime)) gameSettings.IntensifyTime = newIntensifyTime;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intensify Amount");
            intensifyAmount = GUILayout.TextField(intensifyAmount);
            if (System.Double.TryParse(intensifyAmount, out newIntensifyAmount)) gameSettings.IntensifyAmount = (float) newIntensifyAmount;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Point Lead to Win");
            pointLeadToWin = GUILayout.TextField(pointLeadToWin);
            if (System.Int32.TryParse(pointLeadToWin, out newHeroesPoints)) points.PointLeadToWin = newHeroesPoints;
            GUILayout.EndHorizontal();

            // Score System
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Optimal Score per Second");
            optimalScorePerSecond = GUILayout.TextField(optimalScorePerSecond);
            if (System.Int32.TryParse(optimalScorePerSecond, out newOptimalScorePerSecond)) gameSettings.OptimalScorePerSecond = newOptimalScorePerSecond;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Optimal Normal Damage");
            optimalDamage = GUILayout.TextField(optimalDamage);
            if (System.Int32.TryParse(optimalDamage, out newOptimalDamage)) gameSettings.DamageScoreCategories.Find(x => x.name == "DamageDone").optimalValue = newOptimalScorePerSecond;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Optimal Critical Damage");
            optimalCritDamage = GUILayout.TextField(optimalCritDamage);
            if (System.Int32.TryParse(optimalCritDamage, out newOptimalCritDamage)) gameSettings.DamageScoreCategories.Find(x => x.name == "CritDamageDone").optimalValue = newOptimalCritDamage;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Optimal Shielding Percentage");
            optimalShieldingPercentage = GUILayout.TextField(optimalShieldingPercentage);
            if (System.Int32.TryParse(optimalShieldingPercentage, out newOptimalShieldingPercentage)) gameSettings.TankScoreCategories.Find(x => x.name == "DamageShielded").optimalValue = newOptimalShieldingPercentage;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Optimal Boss Orb Hits");
            optimalBossOrbHits = GUILayout.TextField(optimalBossOrbHits);
            if (System.Int32.TryParse(optimalBossOrbHits, out newOptimalBossOrbHits)) gameSettings.RunnerScoreCategories.Find(x => x.name == "OrbBossHits").optimalValue = newOptimalBossOrbHits;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Worst Hero Orb Hits");
            worstHeroOrbHits = GUILayout.TextField(worstHeroOrbHits);
            if (System.Int32.TryParse(worstHeroOrbHits, out newWorstHeroOrbHits)) gameSettings.RunnerScoreCategories.Find(x => x.name == "OrbHeroHits").worstValue = newWorstHeroOrbHits;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
