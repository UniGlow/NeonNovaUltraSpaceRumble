using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Hero : Player {

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Damage")]
    [SerializeField] int damagePerShot = 10;
    [SerializeField] float attackCooldown = 0.2f;
    [SerializeField] float projectileSpeed = 10f;

    [Header("Tank")]
    [SerializeField] float defendCooldown = 3f;

    [Header("Opfer")]
    [Tooltip("Proportional to base movement speed")]
    [SerializeField] float speedBoost = 0.5f;

    [Header("Properties")]
    [SerializeField] PlayerColor playerColor;
    public Ability ability;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject wobbleBobble;

    private bool cooldown = true;

    public int PlayerNumber { get { return playerNumber; } }
    #endregion


    
    #region Unity Event Functions
    // Update is called once per frame
    override protected void Update() {
        base.Update();

        HandleAbilities();
    }
    #endregion



    #region Custom Event Functions
    protected override void OnLevelCompleted() {
        
    }
    #endregion



    #region Public Funtcions

    #endregion



    #region Private Functions
    private void HandleAbilities() {
        if (cooldown) {
            if (ability == Ability.Opfer) {
                Run();
            }
            else if (ability == Ability.Damage && Input.GetButton(Constants.INPUT_ABILITY + playerNumber)) {
                Attack();
            }
            else if (ability == Ability.Tank && Input.GetButtonDown(Constants.INPUT_ABILITY + playerNumber)) {
                Defend();
            }
        }
    }

    private void Attack() {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.GetComponent<HeroProjectile>().damage = damagePerShot;
        projectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;
        cooldown = false;
        StartCoroutine(ResetAttackCooldown());
    }

    private void Defend() {
        wobbleBobble.SetActive(true);
        cooldown = false;
        StartCoroutine(ResetDefendCooldown());
    }

    private void Run() {
        horizontalInput *= (speedBoost + 1);
        verticalInput *= (speedBoost + 1);
    }
    #endregion



    #region Coroutines
    IEnumerator ResetAttackCooldown() {
        yield return new WaitForSecondsRealtime(attackCooldown);
        cooldown = true;
    }

    IEnumerator ResetDefendCooldown() {
        yield return new WaitForSecondsRealtime(defendCooldown);
        wobbleBobble.SetActive(false);
        cooldown = true;
    }
    #endregion
}
