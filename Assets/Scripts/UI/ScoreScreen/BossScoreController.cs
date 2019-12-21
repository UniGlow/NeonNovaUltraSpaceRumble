using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BossScoreController : MonoBehaviour
{
    #region Variable Declarations
    // Serialized Fields
    [SerializeField] TextMeshProUGUI bossCritDamageScore = null;
    [SerializeField] TextMeshProUGUI bossDamageScore = null;
    [SerializeField] TextMeshProUGUI bossShieldedScore = null;
    [SerializeField] Points points = null;
    [SerializeField] float tweeningDuration = 0.7f;

    // Private

    #endregion



    #region Public Properties
    
    #endregion



    #region Unity Event Functions

    #endregion



    #region Public Functions
    public void Clear()
    {
        bossDamageScore.text = "";
        bossCritDamageScore.text = "";
        bossShieldedScore.text = "";
    }

    public void DisplayScoreCategory(ScoreCategory scoreCategory)
    {
        if (scoreCategory.name == "CritDamageDone")
        {
            bossCritDamageScore.transform.localScale = Vector3.zero;
            bossCritDamageScore.text = "+" + points.BossCritDamageInLevels[points.BossCritDamageInLevels.Count - 1].ToString();
            bossCritDamageScore.transform.DOScale(1f, tweeningDuration).SetUpdate(true).SetEase(Ease.OutBounce);
        }
        else if (scoreCategory.name == "DamageDone")
        {
            bossDamageScore.transform.localScale = Vector3.zero;
            bossDamageScore.text = "+" + points.BossDamageInLevels[points.BossDamageInLevels.Count - 1].ToString();
            bossDamageScore.transform.DOScale(1f, tweeningDuration).SetUpdate(true).SetEase(Ease.OutBounce);
        }
        else if (scoreCategory.name == "DamageShielded")
        {
            bossShieldedScore.transform.localScale = Vector3.zero;
            float damageShieldedPercentage = (float)points.BossDamageShieldedInLevels[points.BossDamageShieldedInLevels.Count - 1] / (float)points.BossTotalPointsInLevel;
            bossShieldedScore.text = Mathf.RoundToInt(damageShieldedPercentage * 100).ToString() + "%";
            bossShieldedScore.transform.DOScale(1f, tweeningDuration).SetUpdate(true).SetEase(Ease.OutBounce);
        }
    }
    #endregion



    #region Private Functions

    #endregion
}
