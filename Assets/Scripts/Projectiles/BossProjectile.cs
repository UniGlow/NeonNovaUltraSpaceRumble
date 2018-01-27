using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BossProjectile : Projectile {

    #region Variable Declarations

    #endregion



    #region Unity Event Functions
    override protected void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);

        if (other.tag.Contains(Constants.TAG_SHIELD)) {
            Destroy(gameObject);
        }
        if (other.tag.Contains(Constants.TAG_HERO)) {
            HeroHealth.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
    #endregion
}
