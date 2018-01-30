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
    [SerializeField] int attackDamagePerShot = 10;
    [SerializeField] float attackProjectileSpeed = 20f;
    [SerializeField] float attackProjectileLifeTime = 1f;
    [SerializeField] float attackCooldown = 0.2f;

    [Header("Ability")]
    [SerializeField] int abilityDamagePerShot = 10;
    [SerializeField] int numberOfProjectiles = 20;
    [SerializeField] float abilityProjectileSpeed = 20f;
    [SerializeField] float abilityProjectileLifeTime = 1f;
    [SerializeField] float abilityCooldown = 3f;

    [Header("Properties")]
    [SerializeField] PlayerColor weaknessColor;
    public PlayerColor WeaknessColor { get { return weaknessColor; } }
    [SerializeField] PlayerColor strengthColor;
    public PlayerColor StrengthColor { get { return strengthColor; } }

    [Header("Sound")]
    [SerializeField]
    AudioClip abilitySound;
    [Range(0, 1)]
    [SerializeField]
    float abilitySoundVolume = 1f;
    [SerializeField] AudioClip attackSound;
    [Range(0, 1)]
    [SerializeField]
    float attackSoundVolume = 1f;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Renderer bossMeshRenderer;
    [SerializeField] Material greenBossMat;
    [SerializeField] Material redBossMat;
    [SerializeField] Material blueBossMat;

    private bool attackCooldownB = true;
    private bool abilityCooldownB = true;
    private Color activeStrengthColor;
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
    }
    #endregion



    #region Public Funtcions
    public void SetWeaknessColor(PlayerColor playerColor) {
        weaknessColor = playerColor;

        if (weaknessColor == PlayerColor.Blue) bossMeshRenderer.material = blueBossMat;
        else if (weaknessColor == PlayerColor.Green) bossMeshRenderer.material = greenBossMat;
        else if (weaknessColor == PlayerColor.Red) bossMeshRenderer.material = redBossMat;
    }

    public void SetStrengthColor(PlayerColor playerColor) {
        strengthColor = playerColor;

        if (strengthColor == PlayerColor.Blue) activeStrengthColor = GameManager.Instance.BluePlayerColor;
        else if (strengthColor == PlayerColor.Green) activeStrengthColor = GameManager.Instance.GreenPlayerColor;
        else if (strengthColor == PlayerColor.Red) activeStrengthColor = GameManager.Instance.RedPlayerColor;
    }
    #endregion



    #region Private Functions
    private void Attack() {
        if (Input.GetButton(Constants.INPUT_ABILITY + playerNumber) && attackCooldownB) {
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
        if (Input.GetButtonDown(Constants.INPUT_TRANSMIT + playerNumber) && abilityCooldownB) {

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

            abilityCooldownB = false;
            StartCoroutine(ResetAbilityCooldown());
        }
    }
    #endregion



    #region Coroutines
    IEnumerator ResetAttackCooldown() {
        yield return new WaitForSecondsRealtime(attackCooldown);
        attackCooldownB = true;
    }

    IEnumerator ResetAbilityCooldown() {
        yield return new WaitForSecondsRealtime(abilityCooldown);
        abilityCooldownB = true;
    }
    #endregion
}
