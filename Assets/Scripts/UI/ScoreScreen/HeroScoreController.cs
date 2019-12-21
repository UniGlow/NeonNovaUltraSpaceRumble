using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class HeroScoreController : MonoBehaviour
{
    #region Variable Declarations
    // Serialized Fields
    [SerializeField] TextMeshProUGUI playerColor = null;
    [SerializeField] TextMeshProUGUI score = null;
    [SerializeField] Transform scorePanel = null;
    [SerializeField] float availableTotalWidth = 2000f;
    [SerializeField] float tweeningDuration = 0.7f;

    [Header("References")]
    [SerializeField] PlayerConfig playerConfig = null;
    [SerializeField] Points points = null;
    [SerializeField] GameObject scoreCategoryBarPrefab = null;
    [SerializeField] GameObject scoreCategorySeperatorPrefab = null;

    // Private
    List<ScoreCategoryResult> totalPoints = new List<ScoreCategoryResult>();

    List<ScoreCategoryResult> currentLevelPoints = new List<ScoreCategoryResult>();
    int totalScore;
    int totalScoreWithoutCurrentLevel;
    #endregion



    #region Public Properties
    public PlayerConfig PlayerConfig { get { return playerConfig; } }
    public int TotalScore { get { return totalScore; } }
    #endregion



    #region Unity Event Functions
    private void Start()
    {
        playerColor.text = playerConfig.ColorConfig.name;
        playerColor.color = playerConfig.ColorConfig.uiElementColor;
    }
    #endregion



    #region Public Functions
    public void DisplayTotalPoints(bool animate, bool excludeCurrentLevel)
    {
        if (excludeCurrentLevel)
        {
            DrawPreviousScoreBars();
            DisplayScore(totalScoreWithoutCurrentLevel, null, animate);
        }
        else
        {
            DisplayScore(totalScore, null, animate);
        }
    }

    public void DisplayScoreCategory(ScoreCategory scoreCategory)
    {
        DisplayScore(currentLevelPoints.Find(x => x.scoreCategory == scoreCategory).result, scoreCategory, true);
    }

    public void CalculateTotalPoints()
    {
        totalScore = 0;

        // Calculate total points
        totalPoints.Clear();
        totalPoints = playerConfig.HeroScore.GetScore();
        foreach (ScoreCategoryResult scoreForCategoryResult in totalPoints)
        {
            // HACK: Division by 2 shouldn't be necessary here. There is an error somewhere in the score calculation of the tank
            if (scoreForCategoryResult.scoreCategory.name == "DamageShielded") totalScore += (scoreForCategoryResult.result / 2);
            else totalScore += scoreForCategoryResult.result;
        }

        // Inform the points for check for leading hero
        points.PointsForLeadingHero = totalScore;

        // Subtract the points gained in the current level
        currentLevelPoints.Clear();
        currentLevelPoints = playerConfig.HeroScore.CurrentLevelScore.GetScore();
        if (PlayerConfig.HeroScore.LevelScores.Count > 1)
        {
            totalScoreWithoutCurrentLevel = totalScore;
            foreach (ScoreCategoryResult currentScoreCategoryResult in currentLevelPoints)
            {
                totalScoreWithoutCurrentLevel -= currentScoreCategoryResult.result;
            }
        }
        else totalScoreWithoutCurrentLevel = 0;
    }
    #endregion



    #region Private Functions

    void DisplayScore(int score, ScoreCategory scoreCategory, bool animate)
    {
        // Show Text
        if (animate) this.score.transform.localScale = Vector3.zero;

        if (scoreCategory != null) this.score.text = "+" + score.ToString();
        else this.score.text = score.ToString();

        if (animate) this.score.transform.DOScale(1f, tweeningDuration).SetUpdate(true).SetEase(Ease.OutBounce);

        // Animate bar
        if (animate && scoreCategory != null && score != 0)
        {
            float widthPerPoint = GetAvailableTotalWidth() / (float) points.PointsForLeadingHero;

            GameObject scoreBar = Instantiate(scoreCategoryBarPrefab, scorePanel);
            scoreBar.GetComponent<Image>().color = scoreCategory.color;
            RectTransform rectTransform = scoreBar.GetComponent<RectTransform>();
            rectTransform.DOSizeDelta(new Vector2(score * widthPerPoint, rectTransform.sizeDelta.y), tweeningDuration).SetUpdate(true);
        }
    }

    void DrawPreviousScoreBars()
    {
        float widthPerPoint = GetAvailableTotalWidth() / (float) points.PointsForLeadingHero;

        foreach (LevelScore levelScore in playerConfig.HeroScore.LevelScores)
        {
            // Exclude current level scores
            if (levelScore == playerConfig.HeroScore.CurrentLevelScore) continue;

            // Draw the bars
            List<ScoreCategoryResult> levelScores = levelScore.GetScore();
            foreach (ScoreCategoryResult scoreForCategory in levelScores)
            {
                GameObject scoreBar = Instantiate(scoreCategoryBarPrefab, scorePanel);
                scoreBar.GetComponent<Image>().color = scoreForCategory.scoreCategory.color;
                RectTransform rectTransform = scoreBar.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(scoreForCategory.result * widthPerPoint, rectTransform.sizeDelta.y);
            }

            // Draw seperator line
            GameObject seperator = Instantiate(scoreCategorySeperatorPrefab, scorePanel);
        }
    }

    float GetAvailableTotalWidth()
    {
        float width = availableTotalWidth - (scoreCategorySeperatorPrefab.GetComponent<RectTransform>().sizeDelta.x * (playerConfig.HeroScore.LevelScores.Count - 1));

        return width;
    }
    #endregion
}
