using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BossProjectile : Projectile
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

        if (other.tag.Contains(Constants.TAG_SHIELD))
        {
            other.transform.GetComponentInParent<Hero>().PlayerConfig.ability.AddEnergy(-damage);
            Destroy(gameObject);
        }

        if (other.tag == Constants.TAG_HERO)
        {
            if (playerColor == other.transform.parent.GetComponent<Hero>().PlayerConfig.ColorConfig)
            {
                points.ScorePoints(Faction.Boss, Mathf.RoundToInt(damage * gameSettings.CritDamageMultiplier));
                Instantiate(critHitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(critHitSound, critHitVolume);
            }
            else
            {
                points.ScorePoints(Faction.Boss, damage);
                Instantiate(hitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(hitSound, hitVolume);
            }

            Destroy(gameObject);
        }
    }
    #endregion



    #region Public Functions
    public override void Initialize(int damage, PlayerColor color, Vector3 velocity, float lifeTime = 1)
    {
        base.Initialize(damage, color, velocity, lifeTime);

        GetComponent<Renderer>().material.SetColor("_TintColor", color.bossProjectileColor);
    }
    #endregion
}
