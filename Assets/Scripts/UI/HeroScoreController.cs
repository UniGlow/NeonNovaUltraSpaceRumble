using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroScoreController : MonoBehaviour
{
    #region Variable Declarations
    // Serialized Fields
    [SerializeField] TextMeshProUGUI score = null;
    [SerializeField] HeroScore heroScore = null;

    // Private
    int currentScore;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions

    #endregion



    #region Public Functions
    public void DisplayTotalPoints()
    {
        UpdateTotalPoints();
        score.text = currentScore.ToString();
    }
    #endregion



    #region Private Functions
    void UpdateTotalPoints()
    {
        int totalScore = 0;

        Dictionary<ScoreCategory, int> scoresDict = heroScore.GetScore();
        foreach (KeyValuePair<ScoreCategory, int> scoreCategory in scoresDict)
        {
            totalScore += scoreCategory.Value;
        }

        currentScore = totalScore;
    }
    #endregion
}
