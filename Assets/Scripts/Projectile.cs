using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Projectile : SubscribedBehaviour {
	
	#region Variable Declarations
    [HideInInspector] public int damage;
	#endregion
	
	
	
	#region Unity Event Functions
    private void OnTriggerEnter(Collider other) {
        if (other.tag.Contains(Constants.TAG_BOSS)) {
            print("hit");
            Destroy(gameObject);
        }
    }
    #endregion
}
