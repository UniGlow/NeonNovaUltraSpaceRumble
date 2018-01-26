using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : SubscribedBehaviour
{

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Movement")]
    [SerializeField] float movementSpeed = 10;
    [SerializeField] float rotateSpeed = 1000;

    [Header("Damage")]
    [SerializeField] float attackCooldown = 0.2f;
    [SerializeField] float projectileSpeed = 10f;

    [Header("Tank")]
    [SerializeField] float defendCooldown = 3f;

    [Header("Opfer")]
    [SerializeField] float speedBoost = 1.5f;

    [Header("Properties")]
    [SerializeField] PlayerColor playerColor;
    [SerializeField] Ability ability;
    [SerializeField] int playerNumber;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab;

    // Movement Variables
    private float horizontalInput;
    private float verticalInput;
    private float horizontalLookInput;
    private float verticalLookInput;
    private bool movable = true;

    private bool cooldown = true;

    // Component References
    private new Rigidbody rigidbody;
    #endregion


    
    #region Unity Event Functions
    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MoveCharacter();
        RotateCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        if (movable) {
            horizontalInput = Input.GetAxis(Constants.INPUT_HORIZONTAL + playerNumber) * movementSpeed;
            verticalInput = Input.GetAxis(Constants.INPUT_VERTICAL + playerNumber) * movementSpeed;
            horizontalLookInput = Input.GetAxis(Constants.INPUT_LOOK_HORIZONTAL + playerNumber) * movementSpeed;
            verticalLookInput = Input.GetAxis(Constants.INPUT_LOOK_VERTICAL + playerNumber) * movementSpeed;
        }

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
    /// <summary>
    /// Sets if the character is allowed to move or not and stops his current movement
    /// </summary>
    /// <param name="movable">Character allowed to move?</param>
    public void SetMovable(bool movable) {
        this.movable = movable;
        if (!movable) {
            horizontalInput = 0;
            verticalInput = 0;
        }
    }
    #endregion



    #region Private Functions
    /// <summary>
    /// Sets the velocity of the characters Rigidbody component
    /// </summary>
    private void MoveCharacter() {
        Vector3 newVelocity = rigidbody.velocity;
        newVelocity.x = horizontalInput;
        newVelocity.z = verticalInput;
        rigidbody.velocity = newVelocity;
    }

    private void RotateCharacter() {
        if (horizontalLookInput != 0 || verticalLookInput != 0) {
            Vector3 lookDirection = new Vector3(horizontalLookInput, 0f, verticalLookInput);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), Time.deltaTime * rotateSpeed);
        }
    }

    private void Attack() {
        if (Input.GetButton(Constants.INPUT_ABILITY + playerNumber) && cooldown) {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
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
