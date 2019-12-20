using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HeroScoreController : MonoBehaviour
{
    #region Variable Declarations
    // Serialized Fields
    [SerializeField] TextMeshProUGUI score = null;
    [SerializeField] HeroScore heroScore = null;
    [SerializeField] float availableTotalWidth = 2000f;
    [SerializeField] float tweeningDuration = 0.7f;

    // Private
    Dictionary<ScoreCategory, int> scoresDict = new Dictionary<ScoreCategory, int>();
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions

    #endregion



    #region Public Functions
    public void DisplayTotalPoints(bool animate, bool excludeCurrentLevel)
    {
        if (animate) DisplayScore(CalculateTotalPoints(excludeCurrentLevel).ToString());
        else score.text = CalculateTotalPoints(excludeCurrentLevel).ToString();
    }

    public void DisplayScoreCategory(ScoreCategory scoreCategory)
    {
        DisplayScore("+" + scoresDict[scoreCategory].ToString());
    }
    #endregion



    #region Private Functions    
    /// <summary>
    /// Calculates the total points.
    /// </summary>
    /// <returns></returns>
    int CalculateTotalPoints(bool excludeCurrentLevel)
    {
        int totalScore = 0;

        // Calculate total points
        scoresDict.Clear();
        scoresDict = heroScore.GetScore();
        foreach (KeyValuePair<ScoreCategory, int> scoreForCategory in scoresDict)
        {
            totalScore += scoreForCategory.Value;
        }

        // Subtract the points gained in the current level
        if (excludeCurrentLevel)
        {
            Dictionary<ScoreCategory, int> currentLevelDict = heroScore.CurrentLevelScore.GetScore();
            foreach (KeyValuePair<ScoreCategory, int> currentLevelDictEntry in currentLevelDict)
            {
                totalScore -= currentLevelDictEntry.Value;
            }
        }

        return totalScore;
    }

    void DisplayScore(string text)
    {
        score.transform.localScale = Vector3.zero;
        score.text = text;
        score.transform.DOScale(1f, tweeningDuration).SetUpdate(true).SetEase(Ease.OutBounce);
    }
    #endregion
}
