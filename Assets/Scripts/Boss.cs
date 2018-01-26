using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Boss : Player {

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Properties")]
    [SerializeField] PlayerColor weaknessColor;
    [SerializeField] PlayerColor strengthColor;

    private bool cooldown = true;
    #endregion



    #region Unity Event Functions
    // Update is called once per frame
    override protected void Update() {
        base.Update();

        if (movable) {
            horizontalInput = Input.GetAxis(Constants.INPUT_HORIZONTAL + playerNumber) * movementSpeed;
            verticalInput = Input.GetAxis(Constants.INPUT_VERTICAL + playerNumber) * movementSpeed;
            horizontalLookInput = Input.GetAxis(Constants.INPUT_LOOK_HORIZONTAL + playerNumber) * movementSpeed;
            verticalLookInput = Input.GetAxis(Constants.INPUT_LOOK_VERTICAL + playerNumber) * movementSpeed;
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

    #endregion



    #region Coroutines
    IEnumerator ResetCooldown(float time) {
        yield return new WaitForSecondsRealtime(time);
        cooldown = true;
    }
    #endregion
}
