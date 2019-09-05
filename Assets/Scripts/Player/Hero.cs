using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
public class Hero : Player
{

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Damage")]
    [SerializeField] protected int damagePerShot = 10;
    [SerializeField] protected float attackCooldown = 0.2f;
    [SerializeField] protected float projectileSpeed = 10f;

    [Header("Tank")]
    [SerializeField] protected float defendCooldown = 3f;
    [SerializeField] protected float defendDuration = 2f;

    [Header("Opfer")]
    [Tooltip("Proportional to base movement speed")]
    [SerializeField] protected float speedBoost = 0.5f;

    [Header("Properties")]
    public Ability ability;

    [Header("Sound")]
    [SerializeField]
    protected AudioClip wobbleBobbleSound;
    [Range(0, 1)]
    [SerializeField]
    protected float wobbleBobbleVolume = 1f;
    [SerializeField] protected AudioClip attackSound;
    [Range(0, 1)]
    [SerializeField]
    protected float attackSoundVolume = 1f;

    [Header("References")]
    [SerializeField]
    protected GameObject projectilePrefab;
    [SerializeField] protected GameObject wobbleBobble;
    public SpriteRenderer healthIndicator;
    [SerializeField] protected SpriteRenderer cooldownIndicator;
    public SpriteRenderer CooldownIndicator { get { return cooldownIndicator; } }
    [SerializeField] protected Sprite[] defendCooldownSprites;
    public Sprite TankSprite { get { return defendCooldownSprites[defendCooldownSprites.Length - 1]; } }
    [SerializeField] protected Sprite damageSprite;
    public Sprite DamageSprite { get { return damageSprite; } }
    [SerializeField] protected Sprite opferSprite;
    public Sprite OpferSprite { get { return opferSprite; } }
    [SerializeField] protected Renderer playerMeshRenderer;

    protected bool cooldown = true;
    protected Coroutine resetDefendCoroutine;
    #endregion



    #region Unity Event Functions
    override protected void Start()
    {
        base.Start();
    }

    override protected void Update()
    {
        base.Update();

        if (active)
        {
            HandleAbilities();
        }
    }
    #endregion



    #region Public Funtcions
    /// <summary>
    /// Cancels the ResetDefend Coroutine. Gets called when a transmission happens during reset of the defend ability.
    /// </summary>
    public void CancelDefendReset()
    {
        if (resetDefendCoroutine != null) StopCoroutine(resetDefendCoroutine);
        wobbleBobble.SetActive(false);
        cooldown = true;
    }

    public void SetPlayerConfig(PlayerConfig playerConfig)
    {
        this.playerConfig = playerConfig;

        // Set colors
        playerMeshRenderer.material = playerConfig.ColorConfig.heroMaterial;
        cooldownIndicator.color = playerConfig.ColorConfig.uiElementColor;
        healthIndicator.color = playerConfig.ColorConfig.uiElementColor;

        // TODO
        //SetAbility(playerConfig.ability);
    }

    public void SetAbility(Ability ability)
    {
        this.ability = ability;

        switch (ability)
        {
            case Ability.Damage:
                cooldownIndicator.sprite = DamageSprite;
                break;
            case Ability.Tank:
                cooldownIndicator.sprite = TankSprite;
                break;
            case Ability.Opfer:
                cooldownIndicator.sprite = opferSprite;
                break;
            default:
                break;
        }
    }
    #endregion



    #region Private Functions
    private void HandleAbilities()
    {
        if (cooldown) {
            if (ability == Ability.Opfer)
            {
                Run();
            }
            else if (ability == Ability.Damage && AbilityButtonsDown())
            {
                Attack();
            }
            else if (ability == Ability.Tank && AbilityButtonsDown())
            {
                Defend();
            }
        }
    }

    private bool AbilityButtonsDown()
    {
        if (Input.GetButton(Constants.INPUT_ABILITY + playerConfig.PlayerNumber)) return true;

        return false;
    }

    private void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + Vector3.up * 0.5f, transform.rotation);
        projectile.GetComponent<HeroProjectile>().damage = damagePerShot;
        projectile.GetComponent<HeroProjectile>().playerColor = PlayerConfig.ColorConfig;
        projectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;

        audioSource.PlayOneShot(attackSound, attackSoundVolume);

        cooldown = false;
        StartCoroutine(ResetAttackCooldown());
    }

    private void Defend()
    {
        wobbleBobble.SetActive(true);
        cooldown = false;
        cooldownIndicator.sprite = defendCooldownSprites[0];
        audioSource.PlayOneShot(wobbleBobbleSound, wobbleBobbleVolume);
        resetDefendCoroutine = StartCoroutine(ResetDefend());
    }

    private void Run()
    {
        horizontalInput *= (speedBoost + 1);
        verticalInput *= (speedBoost + 1);
    }
    #endregion



    #region Coroutines
    protected IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        cooldown = true;
    }

    protected IEnumerator ResetDefend()
    {
        // Wait for defend duration and turn of wobbleBobble
        yield return new WaitForSeconds(defendDuration);
        wobbleBobble.SetActive(false);

        // Start Cooldown and update CooldownIndicator
        for (float i = 0; i < defendCooldown; i += Time.deltaTime)
        {
            yield return null;
            cooldownIndicator.sprite = defendCooldownSprites[Mathf.FloorToInt((i / defendCooldown) * defendCooldownSprites.Length)];
        }

        // Reset Cooldown
        cooldown = true;
        resetDefendCoroutine = null;
    }
    #endregion
}
