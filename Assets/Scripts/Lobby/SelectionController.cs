using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SelectionController : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("References")]
    [SerializeField] GameObject wheel = null;
    [SerializeField] GameObject camera = null;
    [SerializeField] List<Transform> models = new List<Transform>();
    [SerializeField] int playerNumber = 0;

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Update()
    {
        
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

