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

        if (hero.PlayerConfig.ability.HasEnergyPool)
        {
            Mathf.Clamp(indicator.fillAmount = hero.PlayerConfig.ability.CurrentEnergy / hero.PlayerConfig.ability.MaxEnergy, 0f, 1f);
        }
        else
        {
            if (hero.PlayerConfig.ability.CooldownVisualized && hero.PlayerConfig.ability.Cooldown != 0)
            {
                Mathf.Clamp(indicator.fillAmount = hero.PlayerConfig.ability.CooldownTimer / hero.PlayerConfig.ability.Cooldown, 0f, 1f);
            }
        }
	}
	#endregion
	
	
	
	#region Public Functions
	public void ResizeRing()
    {
        float x = hero.PlayerConfig.ability.CooldownRingScale;
        Vector3 newScale = new Vector3(x, x, x);
        transform.localScale = newScale;
    }

    public void ResetRing()
    {
        if (!hero.PlayerConfig.ability.CooldownVisualized)
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

