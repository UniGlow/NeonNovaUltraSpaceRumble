using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// 
/// </summary>
public class BossHealth : Health
{

    #region Variable Declarations
    public static BossHealth Instance;
    #endregion



    #region Unity Event Functions
    void Awake()
    {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of an AudioManager.
            Debug.Log("There can only be one BossHealth instantiated. Destroying this Instance...");
            Destroy(this);
        }
    }
    #endregion



    #region Public Functions
    override public void TakeDamage(int damage)
    {
        if (endlessHealth) return;

        base.TakeDamage(damage);

        // Won?
        if (currentDamage >= HeroHealth.Instance.CurrentDamage + HeroHealth.Instance.WinningPointLead)
        {
            RaiseLevelCompleted(Faction.Heroes);

            /*
            Vector3 originalScale = Vector3.one;
            winText.transform.localScale = Vector3.zero;
            winText.text = "Heroes Win !";
            winText.transform.DOScale(originalScale, 0.7f).SetEase(Ease.OutBounce).SetUpdate(true).SetDelay(1f);
            winText.gameObject.SetActive(true);*/
        }
    }
    #endregion



    #region Private Functions
    void RaiseLevelCompleted(Faction winner)
    {
        levelCompletedEvent.Raise(this, winner);
    }
    #endregion
}
