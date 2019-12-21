using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreScreenController : MonoBehaviour
{
    [System.Serializable]
    public class Delay
    {
        public string name;
        public float delay;
    }

    #region Variable Declarations
    // Serialized Fields
    [Header("Timing Settings")]
    [SerializeField] float factionScoreIncreaseDelay = 3f;
    [SerializeField] float roundTimeDelay = 1f;
    [SerializeField] float scoreCategoryFirstDelay = 1f;
    [SerializeField] float scoreCategoryDelay = 2f;
    [SerializeField] float buttonPromptDelay = 3f;

    [SerializeField] float tweeningDuration = 0.7f;

    [Header("Other Settings")]
    [SerializeField] string endScoreText = "";

    [Header("Upper Panels References")]
    [SerializeField] GameObject scoreScreenParent = null;
    [SerializeField] TextMeshProUGUI winner = null;
    [SerializeField] TextMeshProUGUI heroScore = null;
    [SerializeField] TextMeshProUGUI bossScore = null;
    [SerializeField] TextMeshProUGUI roundTime = null;
    [SerializeField] TextMeshProUGUI currentCategory = null;

    [Header("Scores Panels References")]
    [SerializeField] List<HeroScoreController> heroScoreControllers = new List<HeroScoreController>();
    [SerializeField] BossScoreController bossScoreController = null;

    [Header("Out Of Prefab References")]
    [SerializeField] Points points = null;
    [SerializeField] GameSettings gameSettings = null;
    [SerializeField] VersionNumber versionNumber = null;
    [SerializeField] GameEvent scoreScreenFinishedEvent = null;
    [SerializeField] PlayerConfig bossPlayerConfig = null;

    // Private
    Faction winnerOfCurrentRound;
    List<ScoreCategory> scoreCategories = new List<ScoreCategory>();
    DumpFileExport dumpFileExporter;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Awake()
    {
        dumpFileExporter = GetComponent<DumpFileExport>();

        scoreScreenParent.SetActive(false);

        scoreCategories.AddRange(gameSettings.DamageScoreCategories);
        scoreCategories.AddRange(gameSettings.TankScoreCategories);
        scoreCategories.AddRange(gameSettings.RunnerScoreCategories);
    }
    #endregion



    #region Public Functions
    public void ShowScoreScreen()
    {
        if (!gameSettings.UseEndScores) return;

        winnerOfCurrentRound = points.WinningFactions[points.WinningFactions.Count - 1];

        // Display the scores of previous rounds
        if (winnerOfCurrentRound == Faction.Boss)
        {
            bossScore.text = (points.BossWins - 1).ToString();
            heroScore.text = points.HeroWins.ToString();
        }
        else if (winnerOfCurrentRound == Faction.Heroes)
        {
            bossScore.text = points.BossWins.ToString();
            heroScore.text = (points.HeroWins - 1).ToString();
        }

        // Set all Hero Scores to values previous to this round
        foreach (HeroScoreController heroScoreController in heroScoreControllers)
        {
            // Calculate total points to have the leading hero available (necessary for bar scales)
            heroScoreController.CalculateTotalPoints();
            // Then display the points without the current level
            heroScoreController.DisplayTotalPoints(false, true);
        }

        // Set round time, but hide it for now
        roundTime.transform.localScale = Vector3.zero;
        int roundTimeMinutes = Mathf.FloorToInt(points.LevelTimes[points.LevelTimes.Count - 1] / 60);
        int roundTimeSeconds = Mathf.RoundToInt(((points.LevelTimes[points.LevelTimes.Count - 1]) % 60));
        if (roundTimeSeconds < 10) roundTime.text = roundTimeMinutes.ToString() + ":0" + roundTimeSeconds.ToString();
        else roundTime.text = roundTimeMinutes.ToString() + ":" + roundTimeSeconds.ToString();

        // Reset anything else
        bossScoreController.Clear();
        currentCategory.text = "";

        // Show the screen
        scoreScreenParent.SetActive(true);

        // Let loose the tweening madness
        if (winnerOfCurrentRound == Faction.Boss)
        {
            bossScore.transform.DOScale(0f, 0.1f).SetUpdate(true).SetDelay(factionScoreIncreaseDelay).OnComplete(() =>
            {
                bossScore.text = points.BossWins.ToString();
                bossScore.transform.DOScale(1f, tweeningDuration).SetEase(Ease.OutBounce).SetUpdate(true).OnStart(() =>
                {
                    roundTime.transform.DOScale(1f, tweeningDuration).SetEase(Ease.OutBounce).SetUpdate(true).SetDelay(roundTimeDelay).OnStart(() =>
                    {
                        // Start the score categories orchestra
                        StartCoroutine(DisplayScoreCategories());
                    });
                });
            });
        }
        else if (winnerOfCurrentRound == Faction.Heroes)
        {
            // Hide Hero Score
            heroScore.transform.DOScale(0f, 0.1f).SetUpdate(true).SetDelay(factionScoreIncreaseDelay).OnComplete(() =>
            {
                // Show new Hero Score
                heroScore.text = points.HeroWins.ToString();
                heroScore.transform.DOScale(1f, tweeningDuration).SetEase(Ease.OutBounce).SetUpdate(true).OnStart(() =>
                {
                    // Show round time
                    roundTime.transform.DOScale(1f, tweeningDuration).SetEase(Ease.OutBounce).SetUpdate(true).SetDelay(roundTimeDelay).OnStart(() =>
                    {
                        // Start the score categories orchestra
                        StartCoroutine(DisplayScoreCategories());
                    });
                });
            });
        }
    }
    #endregion



    #region Private Functions
    void DisplayWinner()
    {
        // TODO: Display Winner

        dumpFileExporter.CreateDumpFileEntry(heroScoreControllers[0].PlayerConfig, heroScoreControllers[1].PlayerConfig, heroScoreControllers[2].PlayerConfig, bossPlayerConfig, points.LevelTimes, points, gameSettings, versionNumber);
    }
    #endregion



    #region Coroutines
    IEnumerator DisplayScoreCategories()
    {
        yield return new WaitForSecondsRealtime(scoreCategoryFirstDelay);
        foreach (ScoreCategory scoreCategory in scoreCategories)
        {
            currentCategory.transform.localScale = Vector3.zero;
            currentCategory.text = scoreCategory.displayName;
            currentCategory.transform.DOScale(1f, tweeningDuration).SetUpdate(true).SetEase(Ease.OutBounce);
            foreach (HeroScoreController heroScoreController in heroScoreControllers)
            {
                heroScoreController.DisplayScoreCategory(scoreCategory);
            }
            bossScoreController.DisplayScoreCategory(scoreCategory);
            yield return new WaitForSecondsRealtime(scoreCategoryDelay);
        }

        // Show end scores
        currentCategory.transform.localScale = Vector3.zero;
        currentCategory.text = endScoreText;
        currentCategory.transform.DOScale(1f, tweeningDuration).SetUpdate(true).SetEase(Ease.OutBounce);
        foreach (HeroScoreController heroScoreController in heroScoreControllers) heroScoreController.DisplayTotalPoints(true, false);

        yield return new WaitForSecondsRealtime(buttonPromptDelay);
        
        // Announce winner on final score screen
        if (points.BossWins > gameSettings.BestOf / 2)
        {
            winner.transform.localScale = Vector2.zero;
            winner.text = "Boss Wins!";
            winner.transform.DOScale(1f, tweeningDuration).SetUpdate(true).OnComplete(() => 
            {
                winner.transform.DOScale(0.7f, tweeningDuration).SetUpdate(true).SetLoops(99, LoopType.Yoyo);
            });
            RaiseScoreScreenFinished(true);
        }
        else if (points.HeroWins > gameSettings.BestOf / 2)
        {
            winner.transform.localScale = Vector2.zero;
            HeroScoreController winningHero = null;
            foreach (HeroScoreController heroScoreController in heroScoreControllers)
            {
                if (winningHero == null || heroScoreController.TotalScore > winningHero.TotalScore) winningHero = heroScoreController;
            }
            winner.text = winningHero.PlayerConfig.ColorConfig.name + " Hero Wins!";
            winner.transform.DOScale(1f, tweeningDuration).SetUpdate(true).OnComplete(() =>
            {
                winner.transform.DOScale(0.7f, tweeningDuration).SetUpdate(true).SetLoops(99, LoopType.Yoyo);
            });
            RaiseScoreScreenFinished(true);
        }
        else RaiseScoreScreenFinished(false);
    }

    void RaiseScoreScreenFinished(bool finalScoreScreen)
    {
        scoreScreenFinishedEvent.Raise(this, finalScoreScreen);
    }
    #endregion
}
