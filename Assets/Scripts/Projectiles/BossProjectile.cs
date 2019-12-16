using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BossProjectile : Projectile
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

        if (other.tag.Contains(Constants.TAG_SHIELD))
        {
            Hero hero = other.transform.GetComponentInParent<Hero>();
            hero.PlayerConfig.ability.AddEnergy(-damage);
            hero.PlayerConfig.HeroScore.CurrentLevelScore.TankScore.DamageShielded(damage);
            points.UpdateBossPoints(damage, true);
            Destroy(gameObject);
        }

        if (other.tag == Constants.TAG_HERO)
        {
            if (playerConfig == other.transform.parent.GetComponent<Hero>().PlayerConfig.ColorConfig)
            {
                points.ScorePoints(Faction.Boss, Mathf.RoundToInt(damage * gameSettings.CritDamageMultiplier));
                points.UpdateBossPoints(Mathf.RoundToInt(damage * gameSettings.CritDamageMultiplier), false);
                Instantiate(critHitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(critHitSound, critHitVolume);
            }
            else
            {
                points.ScorePoints(Faction.Boss, damage);
                points.UpdateBossPoints(damage, false);
                Instantiate(hitPS, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                AudioManager.Instance.PlayClip(hitSound, hitVolume);
            }

            Destroy(gameObject);
        }
    }
    #endregion



    #region Public Functions
    public override void Initialize(int damage, PlayerColor playerColor, Vector3 velocity, float lifeTime = 1)
    {
        base.Initialize(damage, playerColor, velocity, lifeTime);

        GetComponent<Renderer>().material.SetColor("_BaseColor", playerColor.bossProjectileColor);
        GetComponent<Renderer>().material.SetColor("_EmissionColor", playerColor.bossProjectileColor);
    }
    #endregion
}
