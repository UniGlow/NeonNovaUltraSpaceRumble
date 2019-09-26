using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AlwaysFaceUp : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields

    // Private
    Quaternion rotation;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    void Awake()
    {
        rotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = rotation;
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

