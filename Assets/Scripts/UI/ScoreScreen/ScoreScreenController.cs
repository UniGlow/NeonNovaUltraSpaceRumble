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

    [Header("Upper Panels References")]
    [SerializeField] GameObject scoreScreenParent = null;
    [SerializeField] TextMeshProUGUI heroScore = null;
    [SerializeField] TextMeshProUGUI bossScore = null;
    [SerializeField] TextMeshProUGUI roundTime = null;
    [SerializeField] TextMeshProUGUI currentCategory = null;

    [Header("Scores Panels References")]
    [SerializeField] TextMeshProUGUI bossDamageScore = null;
    [SerializeField] TextMeshProUGUI bossCritDamageScore = null;
    [SerializeField] TextMeshProUGUI bossShieldedScore = null;
    [SerializeField] List<HeroScoreController> heroScoreControllers = new List<HeroScoreController>();
    //[SerializeField] 

    [Header("Out Of Prefab References")]
    [SerializeField] Points points = null;
    [SerializeField] GameSettings gameSettings = null;
    [SerializeField] List<PlayerConfig> playerConfigs = new List<PlayerConfig>();
    [SerializeField] VersionNumber versionNumber = null;

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
        if (winnerOfCurrentRound == Faction.Boss) bossScore.text = (points.BossWins - 1).ToString();
        if (winnerOfCurrentRound == Faction.Heroes) heroScore.text = (points.HeroWins - 1).ToString();

        // TODO: Display texts ("Red", "Blue", "Green") in correct Hero colors

        // Set all Hero Scores to values previous to this round
        foreach (HeroScoreController heroScoreController in heroScoreControllers) heroScoreController.DisplayTotalPoints(false, true);

        // Clear/hide all other texts
        roundTime.transform.localScale = Vector3.zero;
        // TODO: Someone needs to provide the actual playtime of the round here
        int roundTimeMinutes = Mathf.FloorToInt((Time.timeSinceLevelLoad - 4f) / 60);
        int roundTimeSeconds = Mathf.RoundToInt(((Time.timeSinceLevelLoad - 4f) % 60));
        if (roundTimeSeconds < 10) roundTime.text = roundTimeMinutes.ToString() + ":0" + roundTimeSeconds.ToString();
        else roundTime.text = roundTimeMinutes.ToString() + ":" + roundTimeSeconds.ToString();
        currentCategory.text = "";
        bossDamageScore.text = "";
        bossCritDamageScore.text = "";
        bossShieldedScore.text = "";

        // Show the screen
        scoreScreenParent.SetActive(true);

        // Let loose the tweening madness
        if (winnerOfCurrentRound == Faction.Boss)
        {
            bossScore.transform.DOScale(0f, 0.1f).SetUpdate(true).SetDelay(factionScoreIncreaseDelay).OnComplete(() =>
            {
                bossScore.transform.DOScale(1f, tweeningDuration).SetEase(Ease.OutBounce).SetUpdate(true).SetDelay(factionScoreIncreaseDelay).OnStart(() =>
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

        dumpFileExporter.CreateDumpFileEntry(playerConfigs[0], playerConfigs[1], playerConfigs[2], playerConfigs[3], points.LevelTimes, points, gameSettings, versionNumber);
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
            yield return new WaitForSecondsRealtime(scoreCategoryDelay);
        }

        // Show end scores
        foreach (HeroScoreController heroScoreController in heroScoreControllers) heroScoreController.DisplayTotalPoints(true, false);

        yield return new WaitForSecondsRealtime(buttonPromptDelay);
    }
    #endregion
}
