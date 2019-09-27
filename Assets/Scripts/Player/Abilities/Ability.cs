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
    [SerializeField] protected float cooldown = 0.2f;
    [Tooltip("Percentual movement speed modifier. Proportional to base movement speed")]
    [SerializeField] protected float speedBoost = 0f;
    [SerializeField] protected Mesh mesh = null;

    [Header("Sound")]
    [SerializeField] protected AudioClip soundClip = null;
    [Range(0, 1)]
    [SerializeField] protected float volume = 1f;

    [Header("Cooldown")]
    [SerializeField] protected bool cooldownVisualized = false;
    [SerializeField] protected float cooldownRingScale = 1f;

    // Protected
    protected Hero hero = null;
    protected float cooldownTimer = 0f;
    protected AudioSource audioSource = null;
    protected Rigidbody rigidbody = null;
    protected bool binded = false;
    #endregion



    #region Public Properties
    public float Cooldown { get { return cooldown; } }
    public float CooldownTimer { get { return cooldownTimer; } }
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
    #endregion



    #region Unity Event Functions
    private void OnEnable()
    {
        this.hero = null;
        audioSource = null;
        rigidbody = null;
        binded = false;
    }
    #endregion



    #region Public Functions
    /// <summary>
    /// This needs to be called every Update from the hero
    /// </summary>
    public virtual void Tick(float deltaTime, bool abilityButtonPressed)
    {
        if (cooldownTimer >= cooldown)
            cooldownTimer = cooldown;
        else
            cooldownTimer += deltaTime;

        if (abilityButtonPressed && cooldownTimer >= cooldown)
        {
            TriggerAbility();
            cooldownTimer = 0f;
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

    public abstract void TriggerAbility();
    #endregion



    #region Private Functions

    #endregion
}

