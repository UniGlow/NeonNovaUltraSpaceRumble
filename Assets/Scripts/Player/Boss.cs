using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
public class Boss : Player {

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
    [SerializeField] protected PlayerColor weaknessColor;
    public PlayerColor WeaknessColor { get { return weaknessColor; } }
    [SerializeField] protected PlayerColor strengthColor;
    public PlayerColor StrengthColor { get { return strengthColor; } }
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
    [SerializeField] protected Material greenBossMat;
    [SerializeField] protected Material redBossMat;
    [SerializeField] protected Material blueBossMat;

    protected bool attackCooldownB = true;
    protected bool abilityCooldownB = true;
    protected Color activeStrengthColor;
    protected float abilityAxisInputPrevFrame;
    #endregion



    #region Unity Event Functions
    protected override void Start() {
        base.Start();

        SetStrengthColor(strengthColor);
        SetWeaknessColor(weaknessColor);
    }

    override protected void Update() {
        base.Update();

        if (active)
        {
            horizontalInput = Input.GetAxis(Constants.INPUT_HORIZONTAL + playerNumber) * movementSpeed;
            verticalInput = Input.GetAxis(Constants.INPUT_VERTICAL + playerNumber) * movementSpeed;
            horizontalLookInput = Input.GetAxis(Constants.INPUT_LOOK_HORIZONTAL + playerNumber) * movementSpeed;
            verticalLookInput = Input.GetAxis(Constants.INPUT_LOOK_VERTICAL + playerNumber) * movementSpeed;

            Attack();
            Ability();
        }

        abilityAxisInputPrevFrame = Input.GetAxis(Constants.INPUT_TRANSMIT_AXIS + playerNumber);
    }
    #endregion



    #region Public Funtcions
    public void SetWeaknessColor(PlayerColor playerColor)
    {
        weaknessColor = playerColor;

        if (weaknessColor == PlayerColor.Blue) bossMeshRenderer.material = blueBossMat;
        else if (weaknessColor == PlayerColor.Green) bossMeshRenderer.material = greenBossMat;
        else if (weaknessColor == PlayerColor.Red) bossMeshRenderer.material = redBossMat;

        Color newColor = bossMeshRenderer.material.color;
        LeanTween.value(gameObject, bossMeshRenderer.material.color, bossMeshRenderer.material.color * materialGlowOnSwitch, 0.3f).setEaseInOutQuad().setLoopPingPong(1).setOnUpdate((Color value) =>
        {
            newColor = value;
            bossMeshRenderer.material.color = newColor;
        });
    }

    public void SetStrengthColor(PlayerColor playerColor)
    {
        strengthColor = playerColor;

        if (strengthColor == PlayerColor.Blue) activeStrengthColor = GameManager.Instance.BluePlayerColor;
        else if (strengthColor == PlayerColor.Green) activeStrengthColor = GameManager.Instance.GreenPlayerColor;
        else if (strengthColor == PlayerColor.Red) activeStrengthColor = GameManager.Instance.RedPlayerColor;
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
            projectile.GetComponent<Renderer>().material.SetColor("_TintColor", activeStrengthColor);

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
                projectile.GetComponent<Renderer>().material.SetColor("_TintColor", activeStrengthColor);
            }
            
            audioSource.PlayOneShot(abilitySound, abilitySoundVolume);

            if (enableCameraShake) EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);

            abilityCooldownB = false;
            StartCoroutine(ResetAbilityCooldown());
        }
    }

    private bool AttackButtonsPressed()
    {
        if (Input.GetButton(Constants.INPUT_ABILITY + playerNumber)) return true;

        else if (Input.GetAxis(Constants.INPUT_ABILITY_AXIS + playerNumber) > 0) return true;

        return false;
    }

    private bool AbilityButtonsDown()
    {
        if (Input.GetButtonDown(Constants.INPUT_TRANSMIT + playerNumber)) return true;

        //else if (Input.GetAxis(Constants.INPUT_TRANSMIT_AXIS + playerNumber) > 0 &&
        //    abilityAxisInputPrevFrame == 0)
        //{
        //    return true;
        //}

        return false;
    }
    #endregion



    #region Coroutines
    protected IEnumerator ResetAttackCooldown() {
        yield return new WaitForSecondsRealtime(attackCooldown);
        attackCooldownB = true;
    }

    protected IEnumerator ResetAbilityCooldown() {
        yield return new WaitForSecondsRealtime(abilityCooldown);
        abilityCooldownB = true;
    }
    #endregion
}
