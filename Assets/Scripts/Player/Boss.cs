using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Rewired;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
public class Boss : Character
{

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Attack")]
    [SerializeField] protected int attackDamagePerShot = 10;
    [SerializeField] protected float attackProjectileSpeed = 20f;
    [SerializeField] protected float attackProjectileLifeTime = 1f;
    [SerializeField] protected float attackCooldown = 0.2f;

    [Header("Ability Settings")]
    [SerializeField] protected int abilityDamagePerShot = 10;
    [SerializeField] protected int numberOfProjectiles = 20;
    [SerializeField] protected float abilityProjectileSpeed = 20f;
    [SerializeField] protected float abilityProjectileLifeTime = 1f;

    [Space]
    [SerializeField] protected int numberOfNovas = 10;
    [SerializeField] protected float timeBetweenNovas = 0.2f;
    [SerializeField] protected float consecutiveNovaOffset = 0.1f;

    [Space]
    [SerializeField] protected float abilityCooldown = 3f;
    [Range(0f, 1f)]
    [SerializeField] protected float movementSpeedReduction = 0.5f;

    [Header("Ability FX")]
    [SerializeField] protected float abilityAnnounceDuration = 3f;

    [Header("Rumble")]
    [Range(0f, 1f)]
    [SerializeField] float rumbleStrengthDeep = 1f;
    [Range(0f, 1f)]
    [SerializeField] float rumbleStrengthHigh = 1f;
    [SerializeField] float rumbleDuration = 0.5f;

    [Header("Properties")]
    [SerializeField] float materialGlowOnSwitch = 3f;

    [Header("Sound")]
    [SerializeField]
    protected AudioClip abilitySound = null;
    [Range(0, 1)]
    [SerializeField]
    protected float abilitySoundVolume = 1f;
    [SerializeField] protected AudioClip attackSound = null;
    [Range(0, 1)]
    [SerializeField]
    protected float attackSoundVolume = 1f;
    [SerializeField] AudioClip colorChangeSound = null;
    [Range(0, 1)]
    [SerializeField] float colorChangeSoundVolume = 1f;

    [Header("Camera Shake")]
    [SerializeField] public bool enableCameraShake = true;
    [SerializeField] protected float magnitude = 1f;
    [SerializeField] protected float roughness = 10f;
    [SerializeField] protected float fadeIn = 0.1f;
    [SerializeField] protected float fadeOut = 0.8f;

    [Header("References")]
    [SerializeField] protected GameObject projectilePrefab = null;
    [SerializeField] protected Renderer bossMeshRenderer = null;
    [SerializeField] protected Image cooldownIndicator = null;
    [SerializeField] protected GameEvent bossColorChangedEvent = null;
    [SerializeField] protected GameSettings gameSettings = null;
    [SerializeField] protected GameObject novaAnticipationParticle = null;

    protected bool attackCooldownB = true;
    protected bool abilityCooldownB = true;

    // Color Settings
    protected PlayerColor strengthColor;
    public PlayerColor StrengthColor { get { return strengthColor; } }
    protected ColorSet colorSet = null;

    // Color Change
    protected float colorChangeTimer;
    bool colorChangeSoundPlayed;

    protected float cooldownTimer = 0f;
    protected bool abilityInProgress = false;
    #endregion



    #region Unity Event Functions
    // HACK: This normally gets called via GameEvent (LevelInitialized). 
    // For some reason, it doesn't seem to be called though in the built game. Placing it in Start() here is a hotfix.
    private void Start()
    {
        ResetCooldowns(false);
    }

    override protected void Update()
    {
        base.Update();

        if (active)
        {
            // Ability Cooldown
            if (!abilityCooldownB && !abilityInProgress)
            {
                if (cooldownTimer >= abilityCooldown)
                {
                    cooldownTimer = abilityCooldown;
                    abilityCooldownB = true;
                    cooldownIndicator.fillAmount = 1f;
                }
                else
                {
                    cooldownTimer += Time.deltaTime;
                    cooldownIndicator.fillAmount = cooldownTimer / abilityCooldown;
                }
            }

            colorChangeTimer += Time.deltaTime;

            Shoot();
            Ability();
            HandleColorSwitch();
        }
    }
    #endregion



    #region Public Funtcions

    public void SetStrengthColor(PlayerColor playerColor)
    {
        strengthColor = playerColor;
    }

    public void SetPlayerConfig(PlayerConfig playerConfig, ColorSet colorSet)
    {
        this.playerConfig = playerConfig;
        this.colorSet = colorSet;

        if (playerConfig.Faction == Faction.Boss)
        {
            playerConfig.Player.controllers.maps.layoutManager.ruleSets.Add(ReInput.mapping.GetControllerMapLayoutManagerRuleSetInstance("RuleSetBoss"));
            playerConfig.Player.controllers.maps.layoutManager.Apply();
        }
        else Debug.LogError("Boss' playerConfig has set a wrong Faction.", this);

        // Set color
        bossMeshRenderer.material = playerConfig.ColorConfig.heroMaterial;

        SetStrengthColor(colorSet.GetRandomColor());
        SetWeaknessColor(playerConfig.ColorConfig);

        RaiseBossColorChanged(playerConfig);
    }

    /// <summary>
    /// Randomly changes the boss weakness color
    /// </summary>
    public void ChangeWeaknessColor()
    {
        PlayerColor newWeaknessColor = colorSet.GetRandomColorExcept(playerConfig.ColorConfig);

        playerConfig.ColorConfig = newWeaknessColor;

        SetWeaknessColor(playerConfig.ColorConfig);
    }

    /// <summary>
    /// Extra function to be able to call it via GameEvents.
    /// </summary>
    public void ResetSpeed()
    {
        characterStats.ResetSpeed();
    }

    public override void ResetCooldowns(bool maximum)
    {
        base.ResetCooldowns(maximum);
        if (!maximum)
        {
            cooldownTimer = 0f;
            cooldownIndicator.fillAmount = 0f;
            abilityCooldownB = false;
        }
        else
        {
            cooldownTimer = abilityCooldown;
            cooldownIndicator.fillAmount = 1f;
            abilityCooldownB = true;
        }
    }
    #endregion



    #region Private Functions
    private void Shoot()
    {
        if (playerConfig.Player.GetButton(RewiredConsts.Action.SHOOT) && attackCooldownB && !abilityInProgress)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 1.9f + Vector3.up * 0.5f, transform.rotation);
            projectile.GetComponent<BossProjectile>().Initialize(
                attackDamagePerShot,
                strengthColor,
                transform.forward * attackProjectileSpeed,
                attackProjectileLifeTime);

            audioSource.PlayOneShot(attackSound, attackSoundVolume);

            attackCooldownB = false;
            StartCoroutine(ResetAttackCooldown());
        }
    }

    private void Ability()
    {
        if (playerConfig.Player.GetButtonDown(RewiredConsts.Action.TRIGGER_BOSSABILITY) && abilityCooldownB && !abilityInProgress)
        {
            abilityInProgress = true;

            characterStats.ModifySpeed(characterStats.Speed * (1 - movementSpeedReduction));

            StartCoroutine(PrepareNova(() => 
            {
                StartCoroutine(ShootNovas(numberOfNovas, timeBetweenNovas, () =>
                {
                    ResetSpeed();
                    cooldownTimer = 0f;
                    abilityCooldownB = false;
                    abilityInProgress = false;
                }));
            }));
        }
    }

    void SetWeaknessColor(PlayerColor playerColor)
    {
        playerConfig.ColorConfig = playerColor;

        bossMeshRenderer.material = playerConfig.ColorConfig.bossMaterial;

        bossMeshRenderer.material.DOColor(bossMeshRenderer.material.color * materialGlowOnSwitch, 0.6f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad);

        RaiseBossColorChanged(playerConfig);
    }

    protected void HandleColorSwitch()
    {
        if (colorChangeTimer >= gameSettings.BossColorSwitchInterval - 0.5f && !colorChangeSoundPlayed)
        {
            audioSource.PlayOneShot(colorChangeSound, colorChangeSoundVolume);
            colorChangeSoundPlayed = true;
        }
        // Set new Boss color
        if (colorChangeTimer >= gameSettings.BossColorSwitchInterval)
        {
            ChangeWeaknessColor();

            colorChangeTimer = 0f;
            colorChangeSoundPlayed = false;
        }
    }

    void ShootNova(float offset)
    {
        for (int i = 0; i < numberOfProjectiles; ++i)
        {
            float factor = (i / (float)numberOfProjectiles) * Mathf.PI * 2f + offset;
            Vector3 pos = new Vector3(
                Mathf.Sin(factor) * 1.9f,
                transform.position.y + 0.5f,
                Mathf.Cos(factor) * 1.9f);

            GameObject projectile = Instantiate(projectilePrefab, pos + transform.position, Quaternion.identity);
            projectile.GetComponent<BossProjectile>().Initialize(
            attackDamagePerShot,
            strengthColor,
            (projectile.transform.position - transform.position) * abilityProjectileSpeed,
            abilityProjectileLifeTime);
        }

        audioSource.PlayOneShot(abilitySound, abilitySoundVolume);
        if (enableCameraShake) EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        playerConfig.Player.SetVibration(0, rumbleStrengthDeep, rumbleDuration);
        playerConfig.Player.SetVibration(1, rumbleStrengthHigh, rumbleDuration);
    }
    #endregion



    #region GameEvent Raiser
    virtual protected void RaiseBossColorChanged(PlayerConfig bossConfig)
    {
        bossColorChangedEvent.Raise(this, bossConfig);
    }
    #endregion



    #region Coroutines
    protected IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        attackCooldownB = true;
    }

    IEnumerator ShootNovas (int numberOfNovas, float timeBetweenNovas, System.Action onComplete = null)
    {
        for (int i = 0; i < numberOfNovas; i++)
        {
            ShootNova(consecutiveNovaOffset * i);
            yield return new WaitForSeconds(timeBetweenNovas);
        }

        if (onComplete != null) onComplete.Invoke();
    }

    protected IEnumerator PrepareNova(System.Action onComplete)
    {
        novaAnticipationParticle.SetActive(true);
        yield return new WaitForSeconds(abilityAnnounceDuration);
        novaAnticipationParticle.SetActive(false);

        onComplete.Invoke();
    }
    #endregion
}
