using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 
/// </summary>
public class HeroHealth : Health
{

    #region Variable Declarations
    public static HeroHealth Instance;
    #endregion



    #region Unity Event Functions
    void Awake()
    {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of an AudioManager.
            Debug.Log("There can only be one HeroHealth instantiated. Destroying this Instance...");
            Destroy(this);
        }
    }
    #endregion



    #region Public Functions
    override public void TakeDamage(int damage)
    {
        if (endlessHealth) return;

        base.TakeDamage(damage);
        RaiseDamageTaken(Faction.Heroes, damage);

        // Dead?
        if (currentDamage >= BossHealth.Instance.CurrentDamage + BossHealth.Instance.WinningPointLead)
        {
            RaiseLevelCompleted(Faction.Boss);
        }
    }
    #endregion



    #region GameEvent Raiser
    void RaiseLevelCompleted(Faction winner)
    {
        levelCompletedEvent.Raise(this, winner);
    }
    void RaiseDamageTaken(Faction faction, int amount)
    {
        damageTakenEvent.Raise(this, faction, amount);
    }
    #endregion
}
