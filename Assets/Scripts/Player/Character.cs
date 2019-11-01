using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[SelectionBase]
public class Character : MonoBehaviour
{

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Properties")]
    [SerializeField] protected CharacterStats characterStats = null;
    protected PlayerConfig playerConfig = null;

    // Movement Variables
    protected float horizontalMovement;
    protected float verticalMovement;
    protected float horizontalLook;
    protected float verticalLook;
    protected bool active = true;

    // Component References
    protected new Rigidbody rigidbody;
    protected AudioSource audioSource;
    #endregion



    #region Public Properties
    public PlayerConfig PlayerConfig { get { return playerConfig; } }
    public CharacterStats CharacterStats { get { return characterStats; } }
    #endregion



    #region Unity Event Functions
    // Use this for initialization
    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void FixedUpdate()
    {
        if (active)
        {
            MoveCharacter();
        }

        RotateCharacter();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (active)
        {
            horizontalMovement = playerConfig.Player.GetAxis(RewiredConsts.Action.MOVE_HORIZONTAL) * characterStats.speed;
            verticalMovement = playerConfig.Player.GetAxis(RewiredConsts.Action.MOVE_VERTICAL) * characterStats.speed;
        }
            horizontalLook = playerConfig.Player.GetAxis(RewiredConsts.Action.LOOK_HORIZONTAL) * characterStats.rotationSpeed;
            verticalLook = playerConfig.Player.GetAxis(RewiredConsts.Action.LOOK_VERTICAL) * characterStats.rotationSpeed;
    }
    #endregion



    #region Public Funtcions
    /// <summary>
    /// Sets if the character is allowed to move or not and stops his current movement
    /// </summary>
    /// <param name="movable">Character allowed to move?</param>
    public virtual void SetMovable(bool active)
    {
        this.active = active;

        if (!active)
        {
            horizontalMovement = 0;
            verticalMovement = 0;
            rigidbody.velocity = Vector3.zero;
        }

        try
        {
            GetComponent<Transmission>().enabled = active;
        }
        catch (System.NullReferenceException)
        {

        }
    }

    public virtual void ResetCooldowns() { }
    #endregion



    #region Private Functions
    /// <summary>
    /// Sets the velocity of the characters Rigidbody component
    /// </summary>
    private void MoveCharacter()
    {
        Vector3 newVelocity = rigidbody.velocity;
        newVelocity.x = horizontalMovement;
        newVelocity.z = verticalMovement;
        rigidbody.velocity = newVelocity;
    }

    private void RotateCharacter()
    {
        // Ignore rotational inputs when playing victim
        if (playerConfig.ability && playerConfig.ability.Class == Ability.AbilityClass.Victim && rigidbody.velocity.magnitude != 0f)
        {
            transform.forward = rigidbody.velocity;
        }
        // Set rotation corresponding to look inputs 
        else if (horizontalLook != 0 || verticalLook != 0)
        {
            Vector3 lookDirection = new Vector3(horizontalLook, 0f, verticalLook);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.LookRotation(lookDirection, Vector3.up), 
                Time.deltaTime * characterStats.rotationSpeed);
        }
    }
    #endregion
}
