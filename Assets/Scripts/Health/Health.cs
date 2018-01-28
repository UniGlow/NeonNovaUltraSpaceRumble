using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Health : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] protected int maxHealth = 1000;
    [SerializeField] protected Gradient hpColor;

    [SerializeField] protected TextMeshProUGUI winText;
    [SerializeField] protected float fadeInTime = 2f;

    [Header("Object References")]
    [SerializeField] protected Sprite[] healthbarSprites;

    protected int currentHealth;
    protected AudioSource audioSource;
    #endregion



    #region Unity Event Functions
    virtual protected void Start() {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    virtual protected void Update() {

    }
    #endregion



    #region Public Functions
    virtual public void TakeDamage(int damage) {
        currentHealth -= damage;

        if (currentHealth <= 0) {
            currentHealth = 0;
            GameEvents.StartLevelCompleted();
            GameManager.Instance.NextLevel();
            Time.timeScale = 0.0f;
        }
    }
    #endregion
}
