﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class HeroProjectile : Projectile
{

    #region Variable Declarations
    [SerializeField] GameObject hitPS;
    [SerializeField] AudioClip hitSound;
    [Range(0f, 1f)]
    [SerializeField] float hitVolume = 0.8f;
    [SerializeField] GameObject critHitPS;
    [SerializeField] AudioClip critHitSound;
    [Range(0f, 1f)]
    [SerializeField] float critHitVolume = 0.8f;
    #endregion



    #region Unity Event Functions
    override protected void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag == Constants.TAG_BOSS)
        {
            if (playerColor == other.transform.parent.GetComponent<Boss>().PlayerConfig.ColorConfig)
            {
                BossHealth.Instance.TakeDamage(Mathf.RoundToInt(damage * GameManager.Instance.CritDamageMultiplier));
                Instantiate(critHitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(critHitSound, critHitVolume);
            }
            else
            {
                BossHealth.Instance.TakeDamage(damage);
                Instantiate(hitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(hitSound, hitVolume);
            }


            Destroy(gameObject);
        }

        else if (other.tag == Constants.TAG_BOSS_DUMMY)
        {
            if (playerColor == other.transform.parent.GetComponent<BossTutorialAI>().WeaknessColor)
            {
                BossHealth.Instance.TakeDamage(Mathf.RoundToInt(damage * GameManager.Instance.CritDamageMultiplier));
                Instantiate(critHitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(critHitSound, critHitVolume);
            }
            else
            {
                BossHealth.Instance.TakeDamage(damage);
                Instantiate(hitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(hitSound, hitVolume);
            }


            Destroy(gameObject);
        }
    }
    #endregion
}
