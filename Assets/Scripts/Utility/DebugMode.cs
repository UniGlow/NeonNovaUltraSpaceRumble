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
                SceneManager.Instance.LoadNextLevel();
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

            GUILayout.EndVertical();
        }
    }
}
