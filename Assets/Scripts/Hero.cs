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
    [SerializeField] float speedBoost = 1.5f;

    [Header("Properties")]
    [SerializeField] PlayerColor playerColor;
    [SerializeField] Ability ability;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab;

    private bool cooldown = true;
    #endregion


    
    #region Unity Event Functions
    // Update is called once per frame
    override protected void Update() {
        base.Update();

        if (ability == Ability.Damage) {
            Attack();
        }
        else if (ability == Ability.Tank) {
            Defend();
        }
        else if (ability == Ability.Opfer) {
            Run();
        }
    }
    #endregion



    #region Custom Event Functions
    protected override void OnLevelCompleted() {
        
    }
    #endregion



    #region Public Funtcions
    
    #endregion



    #region Private Functions
    private void Attack() {
        if (Input.GetButton(Constants.INPUT_ABILITY + playerNumber) && cooldown) {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            projectile.GetComponent<Projectile>().damage = damagePerShot;
            projectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;
            cooldown = false;
            StartCoroutine(ResetCooldown(attackCooldown));
        }
    }

    private void Defend() {

    }

    private void Run() {

    }
    #endregion



    #region Coroutines
    IEnumerator ResetCooldown(float time) {
        yield return new WaitForSecondsRealtime(time);
        cooldown = true;
    }
    #endregion
}
