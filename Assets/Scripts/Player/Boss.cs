using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
public class Boss : Player
{

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Attack")]
    [SerializeField] protected int attackDamagePerShot = 10;
    [SerializeField] protected float attackProjectileSpeed = 20f;
    [SerializeField] protected float attackProjectileLifeTime = 1f;
    [SerializeField] protected float attackCooldown = 0.2f;

    [Header("Ability")]
    [SerializeField] protected int abilityDamagePerShot = 10;
    [SerializeField] protected int numberOfProjectiles = 20;
    [SerializeField] protected float abilityProjectileSpeed = 20f;
    [SerializeField] protected float abilityProjectileLifeTime = 1f;
    [SerializeField] protected float abilityCooldown = 3f;

    [Header("Properties")]
    [SerializeField] float materialGlowOnSwitch = 3f;

    [Header("Sound")]
    [SerializeField]
    protected AudioClip abilitySound;
    [Range(0, 1)]
    [SerializeField]
    protected float abilitySoundVolume = 1f;
    [SerializeField] protected AudioClip attackSound;
    [Range(0, 1)]
    [SerializeField]
    protected float attackSoundVolume = 1f;
    [SerializeField] AudioClip colorChangeSound;
    [Range(0, 1)]
    [SerializeField] float colorChangeSoundVolume = 1f;

    [Header("Camera Shake")]
    [SerializeField] public bool enableCameraShake = true;
    [SerializeField] protected float magnitude = 1f;
    [SerializeField] protected float roughness = 10f;
    [SerializeField] protected float fadeIn = 0.1f;
    [SerializeField] protected float fadeOut = 0.8f;

    [Header("References")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Renderer bossMeshRenderer;
    public SpriteRenderer healthIndicator;
    [SerializeField] protected GameEvent bossColorChangedEvent = null;
    [SerializeField] protected GameSettings gameSettings = null;

    protected bool attackCooldownB = true;
    protected bool abilityCooldownB = true;

    // Color Settings
    protected PlayerColor strengthColor;
    public PlayerColor StrengthColor { get { return strengthColor; } }
    protected ColorSet colorSet = null;

    // Color Change
    private float colorChangeTimer;
    bool colorChangeSoundPlayed;
    #endregion



    #region Unity Event Functions
    override protected void Update()
    {
        base.Update();

        if (active)
        {
            colorChangeTimer += Time.deltaTime;

            Attack();
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

        // Set color
        bossMeshRenderer.material = playerConfig.ColorConfig.heroMaterial;

        SetStrengthColor(colorSet.GetRandomColor());
        SetWeaknessColor(playerConfig.ColorConfig);

        RaiseBossColorChanged(playerConfig);
    }
    #endregion



    #region Private Functions
    private void Attack() {
        if (AttackButtonsPressed() && attackCooldownB) {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 1.9f + Vector3.up * 0.5f, transform.rotation);
            projectile.GetComponent<BossProjectile>().damage = attackDamagePerShot;
            projectile.GetComponent<BossProjectile>().playerColor = strengthColor;
            projectile.GetComponent<BossProjectile>().lifeTime = attackProjectileLifeTime;
            projectile.GetComponent<Rigidbody>().velocity = transform.forward * attackProjectileSpeed;
            projectile.GetComponent<Renderer>().material.SetColor("_TintColor", strengthColor.bossProjectileColor);

            audioSource.PlayOneShot(attackSound, attackSoundVolume);

            attackCooldownB = false;
            StartCoroutine(ResetAttackCooldown());
        }
    }

    private void Ability() {
        if (AbilityButtonsDown() && abilityCooldownB) {

            for (int i = 0; i < numberOfProjectiles; ++i) {
                float factor = (i / (float)numberOfProjectiles) * Mathf.PI * 2f;
                Vector3 pos = new Vector3(
                    Mathf.Sin(factor) * 1.9f,
                    transform.position.y + 0.5f,
                    Mathf.Cos(factor) * 1.9f);

                GameObject projectile = Instantiate(projectilePrefab, pos + transform.position, Quaternion.identity);
                projectile.GetComponent<BossProjectile>().damage = abilityDamagePerShot;
                projectile.GetComponent<BossProjectile>().playerColor = strengthColor;
                projectile.GetComponent<BossProjectile>().lifeTime = abilityProjectileLifeTime;
                projectile.GetComponent<Rigidbody>().velocity = (projectile.transform.position - transform.position) * abilityProjectileSpeed;
                projectile.GetComponent<Renderer>().material.SetColor("_TintColor", strengthColor.bossProjectileColor);
            }
            
            audioSource.PlayOneShot(abilitySound, abilitySoundVolume);

            if (enableCameraShake) EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);

            abilityCooldownB = false;
            StartCoroutine(ResetAbilityCooldown());
        }
    }

    private bool AttackButtonsPressed()
    {
        if (Input.GetButton(Constants.INPUT_ABILITY + playerConfig.PlayerNumber)) return true;

        return false;
    }

    private bool AbilityButtonsDown()
    {
        if (Input.GetButtonDown(Constants.INPUT_TRANSMIT + playerConfig.PlayerNumber)) return true;

        return false;
    }

    /// <summary>
    /// Randomly changes the boss weakness color
    /// </summary>
    void ChangeWeaknessColor()
    {
        PlayerColor newWeaknessColor = colorSet.GetRandomColorExcept(playerConfig.ColorConfig);

        playerConfig.ColorConfig = newWeaknessColor;

        SetWeaknessColor(playerConfig.ColorConfig);
    }

    void SetWeaknessColor(PlayerColor playerColor)
    {
        playerConfig.ColorConfig = playerColor;

        bossMeshRenderer.material = playerConfig.ColorConfig.bossMaterial;

        bossMeshRenderer.material.DOColor(bossMeshRenderer.material.color * materialGlowOnSwitch, 0.6f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    void HandleColorSwitch()
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

            RaiseBossColorChanged(playerConfig);

            colorChangeTimer = 0f;
            colorChangeSoundPlayed = false;
        }
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
        yield return new WaitForSecondsRealtime(attackCooldown);
        attackCooldownB = true;
    }

    protected IEnumerator ResetAbilityCooldown() {
        yield return new WaitForSecondsRealtime(abilityCooldown);
        abilityCooldownB = true;
    }
    #endregion
}
