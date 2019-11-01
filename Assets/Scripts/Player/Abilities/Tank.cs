using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// 
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Tank")]
public class Tank : Ability
{

    #region Variable Declarations
    // Serialized Fields
    [Header("Ability Properties")]
    [SerializeField] float shieldDuration = 2f;

    // Private
    bool shieldActive = false;
    float shieldTimer = 0f;
    #endregion



    #region Public Properties

    #endregion



    #region Public Functions
    public override void Tick(float deltaTime, bool abilityButtonPressed)
    {
        if (!shieldActive)
        {
            base.Tick(deltaTime, abilityButtonPressed);
        }
        else
        {
            shieldTimer += deltaTime;

            if (shieldTimer >= shieldDuration)
            {
                DeactivateShield();
            }
        }
    }

    public override void TriggerAbility()
    {
        hero.WobbleBobble.SetActive(true);
        hero.Rigidbody.mass = 100f;
        audioSource.PlayOneShot(soundClip, volume);

        shieldActive = true;
    }

    public void DeactivateShield()
    {
        hero.WobbleBobble.SetActive(false);
        hero.Rigidbody.mass = 1f;
        cooldownTimer = 0f;
        shieldTimer = 0f;
        shieldActive = false;
    }
    #endregion



    #region Private Functions

    #endregion
}

