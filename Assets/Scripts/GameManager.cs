using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the overall flow of the game. This class is a singleton and won't be destroyed when loading a new scene.
/// </summary>
public class GameManager : MonoBehaviour
{

    #region Variable Declarations
    public static GameManager Instance;

    // TODO: Verlagern in SO "GameSettings"
    [HideInInspector] public ColorSet activeColorSet = null;
    [HideInInspector] public bool isInitialized = false;
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
    #endregion



    #region Public Functions

    #endregion



    #region Private Functions
    
    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}
