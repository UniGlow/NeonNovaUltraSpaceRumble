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
    [SerializeField]
    GameObject healthbarForeground;

    private float originalHealthbarScale;

    protected int currentHealth;
    #endregion



    #region Unity Event Functions
    virtual protected void Start() {
        currentHealth = maxHealth;
        originalHealthbarScale = healthbarForeground.transform.localScale.x;
    }

    virtual protected void Update() {

    }
    #endregion



    #region Public Functions
    virtual public void TakeDamage(int damage) {
        currentHealth -= damage;

        // Set new color of the healthbar
        healthbarForeground.GetComponent<UnityEngine.UI.Image>().color = hpColor.Evaluate((float)currentHealth / maxHealth);

        // Rescale the green part of the healthbar
        Vector3 newScale = new Vector3(((float)currentHealth / maxHealth) * originalHealthbarScale, healthbarForeground.transform.localScale.y, healthbarForeground.transform.localScale.z);
        healthbarForeground.transform.localScale = newScale;
    }
    #endregion
}
