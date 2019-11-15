using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// 
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Victim/OrbSpeedBoost")]
public class Victim_OrbSpeedBoost : Ability 
{

    #region Variable Declarations
    // Serialized Fields
    [Tooltip("Percentual speed boost for the orb. 1 = normal speed.")]
    [Range(0f, 5f)]
    [SerializeField] float orbSpeedBoost = 2f;

    // Private
    HomingMissile orb = null;
    float originalOrbSpeed = 0f;
    #endregion



    #region Public Properties

    #endregion



    #region Public Functions
    public override void BindTo(Hero hero)
    {
        base.BindTo(hero);

        orb = FindObjectOfType<HomingMissile>();
        originalOrbSpeed = orb.Speed;
    }

    public override void Tick(float deltaTime, bool abilityButtonPressed)
    {
        base.Tick(deltaTime, abilityButtonPressed);

        if (abilityActive && (!abilityButtonPressed || energyPoolRecharging))
        {
            DeactivateAbility();
        }
    }

    public override void TriggerAbility()
    {
        orb.Speed = originalOrbSpeed * orbSpeedBoost;

        abilityActive = true;
    }

    public override void DeactivateAbility()
    {
        orb.Speed = originalOrbSpeed;

        abilityActive = false;
    }
    #endregion



    #region Private Functions

    #endregion
}

