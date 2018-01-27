using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class HeroProjectile : Projectile {

    #region Variable Declarations

    #endregion



    #region Unity Event Functions
    override protected void OnTriggerEnter(Collider other) {
        if (other.tag.Contains(Constants.TAG_BOSS)) {
            BossHealth.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
    #endregion
}
