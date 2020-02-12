using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class CooldownUpdater : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [SerializeField] private Hero hero = null;
    [SerializeField] private Image indicator = null;
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions
	private void Update () 
	{
        if (!hero.PlayerConfig) return;

        if (hero.PlayerConfig.Ability.HasEnergyPool)
        {
            Mathf.Clamp(indicator.fillAmount = hero.PlayerConfig.Ability.CurrentEnergy / hero.PlayerConfig.Ability.MaxEnergy, 0f, 1f);
        }
        else
        {
            if (hero.PlayerConfig.Ability.CooldownVisualized && hero.PlayerConfig.Ability.Cooldown != 0)
            {
                Mathf.Clamp(indicator.fillAmount = hero.PlayerConfig.Ability.CooldownTimer / hero.PlayerConfig.Ability.Cooldown, 0f, 1f);
            }
        }
	}
	#endregion
	
	
	
	#region Public Functions
	public void ResizeRing()
    {
        float x = hero.PlayerConfig.Ability.CooldownRingScale;
        Vector3 newScale = new Vector3(x, x, x);
        transform.localScale = newScale;
    }

    public void ResetRing()
    {
        if (!hero.PlayerConfig.Ability.CooldownVisualized)
        {
            indicator.fillAmount = 1f;
        }
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

