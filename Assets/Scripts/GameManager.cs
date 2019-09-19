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
    [SerializeField] GameSettings gameSettings = null;

    [Header("References")]
    [SerializeField] Points points = null;

    // TODO: Verlagern in SO "GameSettings"
    [HideInInspector] public ColorSet activeColorSet = null;

    float intensifyTimer;
    readonly float countdownDuration = 4f;
    #endregion



    #region Unity Event Functions

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

    private void Start()
    {
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
    #endregion



    #region Public Functions
    

    public void ResetTimer()
    {
        intensifyTimer = 0f;
    }
    #endregion



    #region Private Functions
    void HandleIntensify()
    {
        if (intensifyTimer >= gameSettings.IntensifyTime)
        {
            // Set new pointLeadToWin
            points.PointLeadToWin = Mathf.RoundToInt(points.PointLeadToWin * (1 - gameSettings.IntensifyAmount));

            intensifyTimer = 0f;
        }
    }

    public void OverrideLevelPointLimits() // Temporal public
    {
        points.PointLeadToWin = gameSettings.WinningPointLead;
    }

    #endregion



    #region Coroutines
    
    #endregion
}
