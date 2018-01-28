﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class HeroProjectile : Projectile {

    #region Variable Declarations
    [SerializeField] GameObject hitPS;
    [SerializeField] GameObject critHitPS;
    #endregion



    #region Unity Event Functions
    override protected void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);

        if (other.tag.Contains(Constants.TAG_BOSS)) {
            if (playerColor == other.GetComponent<Boss>().WeaknessColor) {
                BossHealth.Instance.TakeDamage(Mathf.RoundToInt(damage * GameManager.Instance.CritDamageMultiplier));
                Instantiate(critHitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            }
            else {
                BossHealth.Instance.TakeDamage(damage);
                Instantiate(hitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            }


            Destroy(gameObject);
        }
    }
    #endregion
}
