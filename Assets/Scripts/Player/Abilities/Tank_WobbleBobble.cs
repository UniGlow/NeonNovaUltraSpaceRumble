using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// 
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Tank/WobbleBobble")]
public class Tank_WobbleBobble : Ability
{

    #region Variable Declarations
    // Serialized Fields

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Public Functions
    public override void TriggerAbility()
    {
        hero.Shield.SetActive(true);
        hero.Rigidbody.mass = 100f;
        audioSource.PlayOneShot(soundClip, volume);
    }

    public override void DeactivateAbility()
    {
        hero.Shield.SetActive(false);
        hero.Rigidbody.mass = 1f;
    }
    #endregion



    #region Private Functions

    #endregion
}

