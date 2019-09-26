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
        if (hero.PlayerConfig.ability.Cooldown != 0)
        {
            if (hero.PlayerConfig.ability.CooldownTimer == 0)
            {
                indicator.fillAmount = 0;
            }
            else
            {
                indicator.fillAmount = hero.PlayerConfig.ability.CooldownTimer / hero.PlayerConfig.ability.Cooldown;
            }
        }
	}
	#endregion
	
	
	
	#region Public Functions
	
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

