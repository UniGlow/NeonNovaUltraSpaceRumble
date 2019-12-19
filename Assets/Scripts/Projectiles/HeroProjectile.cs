using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class HeroProjectile : Projectile
{

    #region Variable Declarations
    [SerializeField] GameObject hitPS = null;
    [SerializeField] AudioClip hitSound = null;
    [Range(0f, 1f)]
    [SerializeField] float hitVolume = 0.8f;
    [SerializeField] GameObject critHitPS = null;
    [SerializeField] AudioClip critHitSound = null;
    [Range(0f, 1f)]
    [SerializeField] float critHitVolume = 0.8f;
    #endregion



    #region Unity Event Functions
    override protected void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag == Constants.TAG_BOSS)
        {
            if (base.playerColor == other.transform.parent.GetComponent<Boss>().PlayerConfig.ColorConfig)
            {
                points.ScorePoints(Faction.Heroes, Mathf.RoundToInt(damage * gameSettings.CritDamageMultiplier));
                playerConfig.HeroScore.CurrentLevelScore.DamageScore.CritDamageDone(Mathf.RoundToInt(damage * gameSettings.CritDamageMultiplier));
                Instantiate(critHitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(critHitSound, critHitVolume);
            }
            else
            {
                points.ScorePoints(Faction.Heroes, damage);
                playerConfig.HeroScore.CurrentLevelScore.DamageScore.DamageDone(damage);
                Instantiate(hitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(hitSound, hitVolume);
            }

            Destroy(gameObject);
        }
    }
    #endregion
}
