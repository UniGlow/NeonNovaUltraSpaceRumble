using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class BossHealth : Health {

    #region Variable Declarations
    [SerializeField] SpriteRenderer healthIndicator;

    public static BossHealth Instance;
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

    override protected void Start() {
        base.Start();

        healthIndicator.sprite = healthbarSprites[healthbarSprites.Length-1];
	}
	
	override protected void Update() {
        base.Update();
	}
    #endregion



    #region Public Functions
    override public void TakeDamage(int damage) {
        base.TakeDamage(damage);

        // Update Healthbar
        healthIndicator.sprite = healthbarSprites[Mathf.FloorToInt(((float)currentHealth / (float)maxHealth) * healthbarSprites.Length)];

        // Dead?
        if (currentHealth <= 0) {
            healthIndicator.enabled = false;

            GameEvents.StartLevelCompleted("Heroes");

            winText.text = "Heroes Win !";
            winText.gameObject.SetActive(true);
        }
    }
	#endregion
}
