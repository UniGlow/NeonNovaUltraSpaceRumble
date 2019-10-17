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
    
    #endregion



    #region Unity Event Functions
    override protected void Update()
    {
        if (isTargeted && state == State.Deactivated)
        {
            ChangeState(State.ReadyToTransmit);
        }
    }
    #endregion



    new public void EndTransmission()
    {
        
    }



    #region Private Functions

    #endregion
}
