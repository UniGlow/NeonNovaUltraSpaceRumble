using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// 
[CreateAssetMenu(menuName = "Scriptable Objects/Game Settings/Game Settings")]
public class GameSettings : ScriptableObject 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("Game Properties")]
    [SerializeField] private float critDamageMultiplier = 2f;
    [SerializeField] private float intensifyTime = 24f;
    [Range(0, .9f)]
    [SerializeField] private float intensifyAmount = 0.1f;

    [SerializeField] private bool overrideLevelPointLimits = true;
    [SerializeField] private int heroesWinningPointLead = 600;
    [SerializeField] private int bossWinningPointLead = 600;

    [Header("AI Adjustments")]
    [SerializeField] private int bossWinningSolo = 400;
    [SerializeField] private int bossWinningDuo = 700;
    [SerializeField] private int bossWinningTriple = 700;


    // Private
    private ColorSet activeColorSet = null;
    
    #endregion



    #region Public Properties
    public ColorSet ActiveColorSet { get { return activeColorSet; } }
    public float CritDamageMultiplier { get { return critDamageMultiplier; } }
    public float IntensifyTime { get { return intensifyTime; } }
    public float IntensifyAmount { get { return intensifyAmount; } }
    public bool OverrideLevelPointLimits { get { return overrideLevelPointLimits; } }
    public int HeroesWinningPointLead { get { return heroesWinningPointLead; } }
    public int BossWinningPointLead { get { return bossWinningPointLead; } }
    public int BossWinningSolo { get { return bossWinningSolo; } }
    public int BossWinningDuo { get { return bossWinningDuo; } }
    public int BossWinningTriple { get { return bossWinningTriple; } }
    #endregion

	
	
	#region Public Functions
	public void Initialize(ColorSet colorSet)
    {
        this.activeColorSet = colorSet;
    }
	#endregion
	
	
	
	#region Private Functions

	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

