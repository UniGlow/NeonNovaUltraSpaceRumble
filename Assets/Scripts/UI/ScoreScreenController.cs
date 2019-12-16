using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScreenController : MonoBehaviour
{
    #region Variable Declarations
    // Serialized Fields
    [SerializeField] GameObject scoreScreenParent = null;
    [SerializeField] TextMeshProUGUI heroScore = null;
    [SerializeField] TextMeshProUGUI bossScore = null;
    [SerializeField] TextMeshProUGUI roundTime = null;
    [SerializeField] TextMeshProUGUI currentCategory = null;

    [Space]
    [SerializeField] List<HeroScoreController> heroScoreControllers = new List<HeroScoreController>();
    [SerializeField] TextMeshProUGUI bossDamageScore = null;
    [SerializeField] TextMeshProUGUI bossCritDamageScore = null;
    [SerializeField] TextMeshProUGUI bossShieldedScore = null;

    // Private

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
        // TODO: Update and display current hero and boss scores
        roundTime.text = "";
        currentCategory.text = "";
        foreach (HeroScoreController heroScoreController in heroScoreControllers) heroScoreController.Reset();
        bossDamageScore.text = "";
        bossCritDamageScore.text = "";
        bossShieldedScore.text = "";
        scoreScreenParent.SetActive(true);
    }
    #endregion
}
