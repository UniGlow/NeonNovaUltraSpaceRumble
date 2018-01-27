using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class HeroHealth : Health {

    #region Variable Declarations
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

    override protected void Start() {
        base.Start();
    }

    override protected void Update() {
        base.Update();
    }
    #endregion



    #region Public Functions
    override public void TakeDamage(int damage) {
        base.TakeDamage(damage);

        if (currentHealth <= 0) {
            currentHealth = 0;
            print("Heroes dead!");
        }
    }
    #endregion
}
