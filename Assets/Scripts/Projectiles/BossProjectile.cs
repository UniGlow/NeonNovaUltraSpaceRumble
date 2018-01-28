using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BossProjectile : Projectile {

    #region Variable Declarations
    [SerializeField] GameObject hitPS;
    [SerializeField] GameObject critHitPS;
    #endregion



    #region Unity Event Functions
    override protected void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);

        if (other.tag.Contains(Constants.TAG_SHIELD)) {
            Destroy(gameObject);
        }
        if (other.tag.Contains(Constants.TAG_HERO)) {
            if (playerColor == other.transform.parent.GetComponent<Hero>().PlayerColor) {
                HeroHealth.Instance.TakeDamage(Mathf.RoundToInt(damage * GameManager.Instance.CritDamageMultiplier));
                Instantiate(critHitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            }
            else {
                HeroHealth.Instance.TakeDamage(damage);
                Instantiate(hitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            }

            other.transform.parent.GetComponent<Transmission>().EndTransmission();


            Destroy(gameObject);
        }
    }
    #endregion
}
