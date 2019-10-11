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
    override protected void Start()
    {
        base.Start();

        
    }

    override protected void Update()
    {
        //// End transmission if button is lifted
        //if (Input.GetButtonUp(Constants.INPUT_TRANSMIT + 1))
        //{
        //    EndTransmission();
        //}

        //// Look for a receiver
        //if (!receiverFound && transmissionCooldownB && Input.GetButton(Constants.INPUT_TRANSMIT + 1))
        //{
        //    UpdateLineRenderer();
        //    transmissionLineRenderer.gameObject.SetActive(true);
        //    receiverFound = FindReceiver();
        //}
        //// Continue the transmission when a receiver is found
        //else if (receiverFound && Input.GetButton(Constants.INPUT_TRANSMIT + 1))
        //{
        //    UpdateLineRenderer();
        //    Transmit();
        //}
    }
    #endregion



    new public void EndTransmission()
    {
        
    }



    #region Private Functions
    bool FindReceiver()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hitInfo, transmissionRange, 1 << 8))
        {
            Debug.DrawLine(transform.position + Vector3.up * 0.5f, hitInfo.point, Color.green);
            receivers.Clear();
            receivers.Add(hitInfo.transform.GetComponentInParent<Transmission>());
            //hero.SetMovable(false);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * transmissionRange, Color.red);
            return false;
        }

    }
    #endregion
}
