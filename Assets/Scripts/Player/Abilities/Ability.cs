using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

public abstract class Ability : ScriptableObject 
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
    [Tooltip("Percentual movement speed modifier. Proportional to base movement speed")]
    [SerializeField] protected float speedBoost = 0f;
    [Tooltip("Should the player be able to hold down the ability button to trigger again after cooldown?")]
    [SerializeField] protected bool autofire = false;

    [Header("Sound")]
    [SerializeField] protected AudioClip soundClip = null;
    [Range(0, 1)]
    [SerializeField] protected float volume = 1f;

    [Header("Cooldown")]
    [SerializeField] protected float cooldownRingScale = 1f;

    [Space]
    [SerializeField] protected float cooldown = 0.2f;
    [SerializeField] protected bool cooldownVisualized = false;

    [Space]
    [SerializeField] protected bool hasEnergyPool = false;
    [SerializeField] protected float maxEnergy = 100f;
    [SerializeField] protected float energyLossPerSecond = 1f;

    [Header("References")]
    [SerializeField] protected Mesh mesh = null;

    // Protected
    protected Hero hero = null;
    protected float cooldownTimer = 0f;
    protected AudioSource audioSource = null;
    protected Rigidbody rigidbody = null;
    protected bool binded = false;
    protected float currentEnergy = 0f;
    protected bool energyPoolRecharging = false;
    protected bool abilityActive = false;
    #endregion



    #region Public Properties
    public float Cooldown { get { return cooldown; } }
    public float CooldownTimer { get { return cooldownTimer; } }
    public bool HasEnergyPool { get { return hasEnergyPool; } }
    public float CurrentEnergy { get { return currentEnergy; } }
    public float MaxEnergy { get { return maxEnergy; } }
    public AbilityClass Class { get { return abilityClass; } }
    public float SpeedBoost { get { return speedBoost; } }
    public bool Binded
    {
        get
        {
            if (binded && hero != null)
                return true;
            else if (binded)
            {
                BindTo(null);
                return false;
            }
            else
                return false;
        }
    }
    public Mesh Mesh { get { return mesh; } }
    public bool CooldownVisualized { get { return cooldownVisualized; } }
    public float CooldownRingScale { get { return cooldownRingScale; } }
    public bool Autofire { get { return autofire; } }
    #endregion



    #region Unity Event Functions
    private void OnEnable()
    {
        cooldownTimer = 0f;
        currentEnergy = maxEnergy;
        this.hero = null;
        audioSource = null;
        rigidbody = null;
        binded = false;
    }
    #endregion



    #region Public Functions
    /// <summary>
    /// This is called every Update from the hero
    /// </summary>
    public virtual void Tick(float deltaTime, bool abilityButtonPressed)
    {
        // energy available => triggering ability possible
        if (hasEnergyPool && !energyPoolRecharging)
        {
            // Ability active? => adding or reducing energy over time
            if (abilityActive) AddEnergy(-deltaTime * energyLossPerSecond);
            else AddEnergy(deltaTime * (maxEnergy / cooldown));

            if (abilityButtonPressed && !abilityActive && currentEnergy > 0f)
                TriggerAbility();
        }
        // energy has hit 0 => waiting for cooldown to recharge
        else if (hasEnergyPool && energyPoolRecharging)
        {
            AddEnergy(deltaTime * (maxEnergy / cooldown));

            if (currentEnergy >= maxEnergy)
            {
                currentEnergy = maxEnergy;
                energyPoolRecharging = false;
            }
        }
        // No energy pool => wait for cooldown to trigger ability
        else
        {
            cooldownTimer += deltaTime;

            if (abilityButtonPressed && cooldownTimer >= cooldown)
            {
                TriggerAbility();
                cooldownTimer = 0f;
            }
        }
    }

    public virtual void BindTo(Hero hero)
    {
        this.hero = hero;

        if (hero != null)
        {
            audioSource = hero.AudioSource;
            rigidbody = hero.Rigidbody;
            binded = true;
        }
        else
        {
            audioSource = null;
            rigidbody = null;
            binded = false;
        }
    }
    public void AddEnergy(float amount)
    {
        currentEnergy += amount;

        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        else if (currentEnergy <= 0)
        {
            currentEnergy = 0f;
            cooldownTimer = 0f;
            energyPoolRecharging = true;
        }
    }

    /// <summary>
    /// Resets the cooldowns (time- and energy-based).
    /// </summary>
    /// <param name="maximum">If true, sets cooldowns to maximum values (ready state). If false, sets cooldowns to 0.</param>
    public void ResetCooldowns(bool maximum)
    {
        if (maximum)
        {
            cooldownTimer = cooldown;
            currentEnergy = maxEnergy;
        }
        else
        {
            cooldownTimer = 0f;
            currentEnergy = 0f;
        }
    }

    public abstract void TriggerAbility();
    public abstract void DeactivateAbility();
    #endregion



    #region Private Functions
    
    #endregion
}

