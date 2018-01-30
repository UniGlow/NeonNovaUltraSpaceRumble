using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class Health : MonoBehaviour {

    #region Variable Declarations
    [SerializeField] protected int maxHealth = 1000;
    public int MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
    [SerializeField] protected Gradient hpColor;

    [SerializeField] protected TextMeshProUGUI winText;
    [SerializeField] protected float fadeInTime = 2f;

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
        }
    }
    #endregion
}
