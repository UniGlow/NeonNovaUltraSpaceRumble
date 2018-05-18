using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class Health : MonoBehaviour {

    #region Variable Declarations
    [SerializeField] protected int winningPointLead = 500;
    [SerializeField] protected bool endlessHealth;

    [SerializeField] protected TextMeshProUGUI winText;
    [SerializeField] protected float fadeInTime = 2f;

    protected int currentDamage;
    protected HealthbarUpdater healthbarUpdater;
    
    public int WinningPointLead { get { return winningPointLead; } set { winningPointLead = value; } }
    public TextMeshProUGUI WinText { get { return winText; } }
    public int CurrentDamage { get { return currentDamage; } }
    #endregion



    #region Unity Event Functions
    virtual protected void Start()
    {
        healthbarUpdater = GameObject.FindObjectOfType<HealthbarUpdater>();
    }

    virtual protected void Update()
    {

    }
    #endregion



    #region Public Functions
    virtual public void TakeDamage(int damage)
    {
        if (endlessHealth) return;

        currentDamage += damage;

        healthbarUpdater.UpdateHealthbar(HeroHealth.Instance.CurrentDamage, BossHealth.Instance.CurrentDamage);
    }
    #endregion
}
