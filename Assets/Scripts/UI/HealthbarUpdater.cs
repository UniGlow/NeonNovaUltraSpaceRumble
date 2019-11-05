using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 
/// </summary>
public class HealthbarUpdater : MonoBehaviour 
{

    #region Variable Declarations
    [Space]
    [Range(0f, 2f)]
    [SerializeField] float punchAmountOnHit = 1.2f;
    [SerializeField] float punchDuration = 0.3f;

    [Header("References in Prefab")]
    [SerializeField] RectTransform heroHealthbar = null;
    [SerializeField] RectTransform bossHealthbar = null;
    [SerializeField] RectTransform middleImage = null;

    [Header("General References")]
    [SerializeField] Sprite winBoss = null;
    [SerializeField] Sprite winHeroes = null;
    [SerializeField] Points points = null;

    float neutralWidth;
    float totalWidth;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        neutralWidth = heroHealthbar.sizeDelta.x;
        totalWidth = neutralWidth * 2;
    }
	#endregion
	
	
	
	#region Public Functions
	public void UpdateHealthbar(Faction leadingFaction, int pointLead)
    {
        float percentageToVictory = ((float)pointLead / (float)points.PointLeadToWin);

        // Heroes winning
        if (leadingFaction == Faction.Heroes)
        {
            heroHealthbar.sizeDelta = new Vector2(percentageToVictory * neutralWidth + neutralWidth, heroHealthbar.sizeDelta.y);
            if (heroHealthbar.sizeDelta.x > totalWidth) heroHealthbar.sizeDelta = new Vector2(totalWidth, heroHealthbar.sizeDelta.y);

            bossHealthbar.sizeDelta = new Vector3(totalWidth - heroHealthbar.sizeDelta.x, bossHealthbar.sizeDelta.y);

            middleImage.GetComponent<Image>().sprite = winHeroes;
        }
        // Boss winning
        else
        {
            bossHealthbar.sizeDelta = new Vector2(percentageToVictory * neutralWidth + neutralWidth, bossHealthbar.sizeDelta.y);
            if (bossHealthbar.sizeDelta.x > totalWidth) bossHealthbar.sizeDelta = new Vector2(totalWidth, bossHealthbar.sizeDelta.y);

            heroHealthbar.sizeDelta = new Vector2(totalWidth - bossHealthbar.sizeDelta.x, heroHealthbar.sizeDelta.y);

            middleImage.GetComponent<Image>().sprite = winBoss;
        }

        Vector2 newImagePosition = new Vector2(heroHealthbar.sizeDelta.x - neutralWidth, middleImage.anchoredPosition.y);
        middleImage.anchoredPosition = newImagePosition;
        if (!DOTween.IsTweening(middleImage)) middleImage.DOPunchScale(middleImage.localScale * punchAmountOnHit, punchDuration);
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
}
