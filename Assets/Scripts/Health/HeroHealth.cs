using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class HeroHealth : Health {

    #region Variable Declarations
    public static HeroHealth Instance;

    [Header("Object References")]
    [SerializeField] SpriteRenderer[] healthIndicators;

    private float originalHealthbarScale;
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

        foreach (SpriteRenderer spriteRend in healthIndicators) {
            spriteRend.sprite = healthbarSprites[healthbarSprites.Length - 1];
        }
    }

    override protected void Update() {
        base.Update();
    }
    #endregion



    #region Public Functions
    override public void TakeDamage(int damage) {
        base.TakeDamage(damage);

        // Update Healthbars
        foreach (SpriteRenderer spriteRend in healthIndicators) {
            spriteRend.sprite = healthbarSprites[Mathf.FloorToInt(((float)currentHealth / (float)maxHealth) * healthbarSprites.Length)];
        }

        // Dead?
        if (currentHealth <= 0)
        {
            GameEvents.StartLevelCompleted("Boss");

            winText.text = "Boss Wins !";
            winText.gameObject.SetActive(true);
        }
    }
    #endregion
}
