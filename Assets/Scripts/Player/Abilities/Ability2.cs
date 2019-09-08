using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

public abstract class Ability2 : ScriptableObject 
{
    public enum AbilityClass
    {
        Damage,
        Tank,
        Victim
    }

    #region Variable Declarations
    // Serialized Fields
    [Header("General Properties")]
    [SerializeField] protected AbilityClass abilityClass = AbilityClass.Victim;
    [SerializeField] protected float cooldown = 0.2f;
    [Tooltip("Percentual movement speed modifier. Proportional to base movement speed")]
    [SerializeField] protected float speedBoost = 0f;

    [Header("Sound")]
    [SerializeField] protected AudioClip soundClip = null;
    [Range(0, 1)]
    [SerializeField] protected float volume = 1f;

    // Protected
    protected Hero hero = null;
    protected float cooldownTimer = 0f;
    protected AudioSource audioSource = null;
    protected Rigidbody rigidbody = null;
    #endregion



    #region Public Properties
    public float Cooldown { get { return cooldown; } }
    public AbilityClass Class { get { return abilityClass; } }
    public float SpeedBoost { get { return speedBoost; } }
    #endregion


    /// <summary>
    /// This needs to be called every Update from the hero
    /// </summary>
    public virtual void Update(float deltaTime, bool abilityButtonPressed)
    {
        cooldownTimer += deltaTime;

        if (abilityButtonPressed && cooldownTimer >= cooldown)
        {
            TriggerAbility();
            cooldownTimer = 0f;
        }
    }

    #region Public Functions
    public virtual void BindTo(Hero hero)
    {
        this.hero = hero;
        audioSource = hero.AudioSource;
        rigidbody = hero.Rigidbody;
    }

    public abstract void TriggerAbility();
    #endregion



    #region Private Functions

    #endregion
}

