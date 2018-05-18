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
            Debug.Log("There can only be one HeroHealth instantiated. Destroying this Instance...");
            Destroy(this);
        }
    }

    override protected void Update() {
        base.Update();
    }
    #endregion



    #region Public Functions
    override public void TakeDamage(int damage)
    {
        if (endlessHealth) return;

        base.TakeDamage(damage);

        // Won?
        if (BossHealth.Instance.CurrentDamage >= currentDamage + winningPointLead)
        {
            GameEvents.StartLevelCompleted("Heroes");

            Vector3 originalScale = winText.transform.localScale;
            winText.transform.localScale = Vector3.zero;
            winText.text = "Heroes Win !";
            LeanTween.scale(winText.gameObject, originalScale, 0.7f).setEase(LeanTweenType.easeOutBounce).setIgnoreTimeScale(true).setDelay(1f);
            winText.gameObject.SetActive(true);
        }
    }
    #endregion
}
