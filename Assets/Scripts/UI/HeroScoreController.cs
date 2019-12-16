using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroScoreController : MonoBehaviour
{
    #region Variable Declarations
    // Serialized Fields
    [SerializeField] TextMeshProUGUI score = null;

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions

    #endregion



    #region Public Functions
    public void Reset()
    {
        // TODO: Show scores from last rounds
        score.text = "";
    }
    #endregion
}
