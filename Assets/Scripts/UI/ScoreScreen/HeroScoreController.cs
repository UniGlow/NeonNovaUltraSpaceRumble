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
    [SerializeField] List<PlayerConfig> otherHeroConfigs = new List<PlayerConfig>();
    [SerializeField] Points points = null;
    [SerializeField] GameObject scoreCategoryBarPrefab = null;
    [SerializeField] GameObject scoreCategorySeperatorPrefab = null;

    // Private
    Dictionary<ScoreCategory, int> scoresDictTotalPoints = new Dictionary<ScoreCategory, int>();

    Dictionary<ScoreCategory, int> scoresDictCurrentLevel = new Dictionary<ScoreCategory, int>();
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
        DisplayScore(scoresDictCurrentLevel[scoreCategory], scoreCategory, true);
    }

    public void CalculateTotalPoints()
    {
        totalScore = 0;

        // Calculate total points
        scoresDictTotalPoints.Clear();
        scoresDictTotalPoints = playerConfig.HeroScore.GetScore();
        foreach (KeyValuePair<ScoreCategory, int> scoreForCategory in scoresDictTotalPoints)
        {
            totalScore += scoreForCategory.Value;
        }

        // Inform the points for check for leading hero
        points.PointsForLeadingHero = totalScore;
        Debug.Log(totalScore, gameObject);

        // Subtract the points gained in the current level
        scoresDictCurrentLevel.Clear();
        scoresDictCurrentLevel = playerConfig.HeroScore.CurrentLevelScore.GetScore();
        if (PlayerConfig.HeroScore.LevelScores.Count > 1)
        {
            totalScoreWithoutCurrentLevel = totalScore;
            foreach (KeyValuePair<ScoreCategory, int> currentLevelDictEntry in scoresDictCurrentLevel)
            {
                totalScoreWithoutCurrentLevel -= currentLevelDictEntry.Value;
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
            Dictionary<ScoreCategory, int> levelScores = levelScore.GetScore();
            foreach (KeyValuePair<ScoreCategory, int> scoreForCategory in levelScores)
            {
                GameObject scoreBar = Instantiate(scoreCategoryBarPrefab, scorePanel);
                scoreBar.GetComponent<Image>().color = scoreForCategory.Key.color;
                RectTransform rectTransform = scoreBar.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(scoreForCategory.Value * widthPerPoint, rectTransform.sizeDelta.y);
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
