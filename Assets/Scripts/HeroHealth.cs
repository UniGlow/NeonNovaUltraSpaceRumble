using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class HeroHealth : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] int maxHealth = 1000;
    [SerializeField] Gradient hpColor;

    [Header("Object References")]
    [SerializeField]
    GameObject healthbarForeground;

    private int currentHealth;
    private float originalHealthbarScale;

    public static HeroHealth Instance;
    #endregion



    #region Unity Event Functions
    void Awake() {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this) {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of an AudioManager.
            Debug.Log("There can only be one GameManager instantiated. Destroying this Instance...");
            Destroy(this);
        }
    }

    private void Start() {
        currentHealth = maxHealth;
        originalHealthbarScale = healthbarForeground.transform.localScale.x;
    }

    private void Update() {

    }
    #endregion



    #region Public Functions
    public void TakeDamage(int damage) {
        currentHealth -= damage;

        // Set new color of the healthbar
        healthbarForeground.GetComponent<UnityEngine.UI.Image>().color = hpColor.Evaluate((float)currentHealth / maxHealth);

        // Rescale the green part of the healthbar
        Vector3 newScale = new Vector3(((float)currentHealth / maxHealth) * originalHealthbarScale, healthbarForeground.transform.localScale.y, healthbarForeground.transform.localScale.z);
        healthbarForeground.transform.localScale = newScale;
    }
    #endregion
}
