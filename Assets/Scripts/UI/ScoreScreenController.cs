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
    [SerializeField] float scoreCategoryDelay = 3f;

    [SerializeField] float tweeningDuration = 0.7f;

    [Header("Upper Panels References")]
    [SerializeField] GameObject scoreScreenParent = null;
    [SerializeField] TextMeshProUGUI heroScore = null;
    [SerializeField] TextMeshProUGUI bossScore = null;
    [SerializeField] TextMeshProUGUI roundTime = null;
    [SerializeField] TextMeshProUGUI currentCategory = null;

    [Header("Scores Panels References")]
    [SerializeField] List<HeroScoreController> heroScoreControllers = new List<HeroScoreController>();
    [SerializeField] TextMeshProUGUI bossDamageScore = null;
    [SerializeField] TextMeshProUGUI bossCritDamageScore = null;
    [SerializeField] TextMeshProUGUI bossShieldedScore = null;

    [Header("Out Of Prefab References")]
    [SerializeField] Points points = null;

    // Private
    Faction winnerOfCurrentRound;
    bool scoreScreenActive;
    float activeTimer;
    float elapsedDelays;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Awake()
    {
        scoreScreenParent.SetActive(false);
    }

    private void Update()
    {
        // Break condition
        if (!scoreScreenActive) return;

        // Main Stuff
        

        // Increase timer
        activeTimer += Time.deltaTime;
    }
    #endregion



    #region Public Functions
    public void ShowScoreScreen()
    {
        winnerOfCurrentRound = points.WinningFactions[points.WinningFactions.Count - 1];
        // For now, subtract the winner of this round
        if (winnerOfCurrentRound == Faction.Boss) bossScore.text = (points.BossWins - 1).ToString();
        if (winnerOfCurrentRound == Faction.Heroes) heroScore.text = (points.HeroWins - 1).ToString();

        // Set all Hero Scores to values previous to this round
        foreach (HeroScoreController heroScoreController in heroScoreControllers) heroScoreController.DisplayTotalPoints();

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
        scoreScreenActive = true;
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
                        // TODO: Start the score categories orchestra
                    });
                });
            });
        }
        else if (winnerOfCurrentRound == Faction.Heroes)
        {
            heroScore.transform.DOScale(0f, 0.1f).SetUpdate(true).SetDelay(factionScoreIncreaseDelay).OnComplete(() =>
            {
                heroScore.text = points.HeroWins.ToString();
                heroScore.transform.DOScale(1f, tweeningDuration).SetEase(Ease.OutBounce).SetUpdate(true).OnStart(() =>
                {
                    roundTime.transform.DOScale(1f, tweeningDuration).SetEase(Ease.OutBounce).SetUpdate(true).SetDelay(roundTimeDelay).OnStart(() =>
                    {
                        // TODO: Start the score categories orchestra
                    });
                });
            });
        }
    }
    #endregion
}
