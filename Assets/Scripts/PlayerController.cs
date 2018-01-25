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
    [SerializeField] float movementSpeed = 10;

    // Movement Variables
    private float horizontalInput;
    private float verticalInput;
    private bool movable = true;

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
        MoveCharacter(horizontalInput, verticalInput);
    }

    // Update is called once per frame
    void Update()
    {
        if (movable) {
            horizontalInput = Input.GetAxis(Constants.INPUT_HORIZONTAL) * movementSpeed;
            verticalInput = Input.GetAxis(Constants.INPUT_VERTICAL) * movementSpeed;
        }
    }
    #endregion



    #region Custom Event Functions
    protected override void OnLevelCompleted() {
        
    }
    #endregion



    #region Private Functions
    /// <summary>
    /// Sets the velocity of the characters Rigidbody component
    /// </summary>
    /// <param name="horizontalInput">Movement amount on x axis</param>
    /// <param name="verticalInput">Movement amount on y axis</param>
    private void MoveCharacter(float horizontalInput, float verticalInput) {
        Vector3 newVelocity = rigidbody.velocity;
        newVelocity.x = horizontalInput;
        newVelocity.z = verticalInput;
        rigidbody.velocity = newVelocity;
    }

    /// <summary>
    /// Sets if the character is allowed to move or not and stops his current movement
    /// </summary>
    /// <param name="movable">Character allowed to move?</param>
    private void SetMovable(bool movable) {
        this.movable = movable;
        if (!movable) {
            horizontalInput = 0;
            verticalInput = 0;
        }
    }
    #endregion
}
