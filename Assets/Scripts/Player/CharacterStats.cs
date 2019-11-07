using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

[CreateAssetMenu(menuName = "Scriptable Objects/CharacterStats")]
public class CharacterStats : ScriptableObject 
{

    #region Variable Declarations
    // Serialized Fields
    [SerializeField] float speed = 0f;
    [SerializeField] float rotationSpeed = 0f;

    // Private
    float originalSpeed = 0f;
    #endregion



    #region Public Properties
    public float Speed { get { return speed; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    #endregion



    #region Unity Event Functions
    private void OnEnable()
    {
        originalSpeed = this.speed;
    }

    private void OnDisable()
    {
        ResetSpeed();
    }
    #endregion
    


    #region Public Functions
    public void ModifySpeed(float speed)
    {
        this.speed = speed;
    }

    public void ResetSpeed()
    {
        speed = originalSpeed;
    }
	#endregion
	
	
	
	#region Private Functions

	#endregion
}

