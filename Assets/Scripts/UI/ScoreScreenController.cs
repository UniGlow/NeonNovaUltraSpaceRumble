using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScreenController : MonoBehaviour
{
    #region Variable Declarations
    // Serialized Fields
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
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Awake()
    {
        scoreScreenParent.SetActive(false);
    }
    #endregion



    #region Public Functions
    public void ShowScoreScreen()
    {
        // Set faction scores
        int bossWins = points.BossWins;
        int heroWins = points.HeroWins;
        winnerOfCurrentRound = points.WinningFactions[points.WinningFactions.Count - 1];
        // For now, subtract the winner of this round
        if (winnerOfCurrentRound == Faction.Boss) bossWins -= 1;
        if (winnerOfCurrentRound == Faction.Heroes) heroWins -= 1;
        // Print
        bossScore.text = bossWins.ToString();
        heroScore.text = heroWins.ToString();

        // Set all Hero Scores to values previous to this round
        foreach (HeroScoreController heroScoreController in heroScoreControllers) heroScoreController.DisplayTotalPoints();

        // Clear all other texts
        roundTime.text = "";
        currentCategory.text = "";
        bossDamageScore.text = "";
        bossCritDamageScore.text = "";
        bossShieldedScore.text = "";

        // Show the screen
        scoreScreenParent.SetActive(true);
    }
    #endregion
}
