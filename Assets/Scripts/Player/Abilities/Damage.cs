using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Damage")]
public class Damage : Ability
{

    #region Variable Declarations
    // Serialized Fields
    [Header("Ability Properties")]
    [SerializeField] int damagePerShot = 10;
    [SerializeField] float projectileSpeed = 10f;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab = null;

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Public Functions
    public override void TriggerAbility()
    {
        base.TriggerAbility();

        GameObject projectile = Instantiate(projectilePrefab, hero.transform.position + Vector3.up * 0.5f, hero.transform.rotation);
        projectile.GetComponent<HeroProjectile>().Initialize(damagePerShot, hero.PlayerConfig, hero.transform.forward * projectileSpeed);

        audioSource.PlayOneShot(soundClip, volume);
    }
    #endregion



    #region Private Functions

    #endregion
}

