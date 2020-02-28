using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Hero))]
public class TransmissionAI : Transmission
{

    #region Variable Declarations
    [SerializeField] float failsafeDeactivate = 2f;

    float timer = 0f;
    #endregion



    #region Unity Event Functions
    override protected void Update()
    {
        if (isTargeted && state == State.Deactivated)
        {
            ChangeState(State.ReadyToTransmit);
        }

        else if (state == State.ReadyToTransmit)
        {
            timer += Time.deltaTime;
            if (timer >= failsafeDeactivate)
            {
                timer = 0f;
                ChangeState(State.Deactivated);
            }
        }

    }
    #endregion



    new public void EndTransmission()
    {
        
    }



    #region Private Functions

    #endregion
}
