using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] private float lifeTime = 10f;

    public int damage = 10;
    #endregion



    #region Unity Event Functions
    private void Start() {
        StartCoroutine(DestroyObject());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag.Contains(Constants.TAG_BOSS)) {
            BossHealth.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
    #endregion



    IEnumerator DestroyObject() {
        yield return new WaitForSecondsRealtime(lifeTime);
        Destroy(gameObject);
    }
}
