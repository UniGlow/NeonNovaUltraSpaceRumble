using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class HealthbarUpdater : MonoBehaviour 
{

    #region Variable Declarations
    [SerializeField] RectTransform heroHealthbar;
    [SerializeField] RectTransform bossHealthbar;
    [SerializeField] Image middleImage;

    float neutralWidth;
    float totalWidth;
    float winningPointLeadHeroes;
    float winningPointLeadBoss;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        neutralWidth = heroHealthbar.sizeDelta.x;
        totalWidth = neutralWidth * 2;
        winningPointLeadHeroes = HeroHealth.Instance.WinningPointLead;
        winningPointLeadBoss = BossHealth.Instance.WinningPointLead;
    }
	#endregion
	
	
	
	#region Public Functions
	public void UpdateHealthbar(float heroDamage, float bossDamage)
    {
        if (bossDamage >= heroDamage)
        {
            heroHealthbar.sizeDelta = new Vector2(((bossDamage - heroDamage) / winningPointLeadHeroes) * neutralWidth + neutralWidth, heroHealthbar.sizeDelta.y);
            if (heroHealthbar.sizeDelta.x > totalWidth) heroHealthbar.sizeDelta = new Vector2(totalWidth, heroHealthbar.sizeDelta.y);
            bossHealthbar.sizeDelta = new Vector3(totalWidth - heroHealthbar.sizeDelta.x, bossHealthbar.sizeDelta.y);
        }
        else
        {
            bossHealthbar.sizeDelta = new Vector2(((heroDamage - bossDamage) / winningPointLeadBoss) * neutralWidth + neutralWidth, bossHealthbar.sizeDelta.y);
            if (bossHealthbar.sizeDelta.x > totalWidth) bossHealthbar.sizeDelta = new Vector2(totalWidth, bossHealthbar.sizeDelta.y);
            heroHealthbar.sizeDelta = new Vector2(totalWidth - bossHealthbar.sizeDelta.x, heroHealthbar.sizeDelta.y);
        }
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
}
