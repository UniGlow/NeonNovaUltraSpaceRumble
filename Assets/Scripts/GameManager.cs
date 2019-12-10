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
    private bool isInitialized = false;

    public bool IsInitialized { get { return isInitialized; } set { isInitialized = value; } }
    #endregion



    #region Unity Event Functions
    //Awake is always called before any Start functions
    void Awake()
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
