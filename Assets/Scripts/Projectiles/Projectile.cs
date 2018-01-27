using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Projectile : SubscribedBehaviour {

    #region Variable Declarations
    public float lifeTime = 1f;
    public int damage = 10;
    #endregion



    #region Unity Event Functions
    virtual protected void Start() {
        StartCoroutine(DestroyObject());
    }

    virtual protected void OnTriggerEnter(Collider other) {
        if (other.tag.Contains(Constants.TAG_WALL)) {
            Destroy(gameObject);
        }
    }

    private void Update() {
		
	}
    #endregion



    #region Private Functions
    #endregion



    IEnumerator DestroyObject() {
        yield return new WaitForSecondsRealtime(lifeTime);
        Destroy(gameObject);
    }
}
