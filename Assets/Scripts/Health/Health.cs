using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Health : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] protected int maxHealth = 1000;
    [SerializeField] protected Gradient hpColor;

    [Header("Object References")]
    [SerializeField] protected Sprite[] healthbarSprites;

    protected int currentHealth;
    #endregion



    #region Unity Event Functions
    virtual protected void Start() {
        currentHealth = maxHealth;
    }

    virtual protected void Update() {

    }
    #endregion



    #region Public Functions
    virtual public void TakeDamage(int damage) {
        currentHealth -= damage;

        if (currentHealth <= 0) {
            currentHealth = 0;
            Time.timeScale = 0;
        }
    }
    #endregion
}
