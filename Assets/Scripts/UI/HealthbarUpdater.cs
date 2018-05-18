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
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        neutralWidth = heroHealthbar.sizeDelta.x;
        totalWidth = neutralWidth * 2;
	}
	#endregion
	
	
	
	#region Public Functions
	public void UpdateHealthbar(float heroHealth, float bossHealth)
    {
        heroHealthbar.sizeDelta = new Vector3((heroHealth / (heroHealth + bossHealth)) * totalWidth, heroHealthbar.rect.height);
        bossHealthbar.sizeDelta = new Vector3((bossHealth / (heroHealth + bossHealth)) * totalWidth, bossHealthbar.rect.height);
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
}
