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
    [Space]
    [SerializeField] private float intensifyTime = 24f;
    [Range(0, .9f)]
    [SerializeField] private float intensifyAmount = 0.1f;
    [Space]
    [SerializeField] private bool overrideLevelPointLimits = true;
    [SerializeField] private int winningPointLead = 600;
    [Space]
    [SerializeField] private float bossColorSwitchInterval = 10f;

    [Header("AI Adjustments")]
    [SerializeField] private int bossWinningSolo = 400;
    [SerializeField] private int bossWinningDuo = 700;
    [SerializeField] private int bossWinningTriple = 700;


    // Private
    private ColorSet activeColorSet = null;
    
    #endregion



    #region Public Properties
    public ColorSet ActiveColorSet { get { return activeColorSet; } }
    public float CritDamageMultiplier { get { return critDamageMultiplier; } set { critDamageMultiplier = value; } }
    public float IntensifyTime { get { return intensifyTime; } set { intensifyTime = value; } }
    public float IntensifyAmount { get { return intensifyAmount; } set { intensifyAmount = value; } }
    public bool OverrideLevelPointLimits { get { return overrideLevelPointLimits; } set { overrideLevelPointLimits = value; } }
    public int WinningPointLead { get { return winningPointLead; } set { winningPointLead = value; } }
    public int BossWinningSolo { get { return bossWinningSolo; } set { bossWinningSolo = value; } }
    public int BossWinningDuo { get { return bossWinningDuo; } set { bossWinningDuo = value; } }
    public int BossWinningTriple { get { return bossWinningTriple; } set { bossWinningTriple = value; } }
    public float BossColorSwitchInterval { get { return bossColorSwitchInterval; } set { bossColorSwitchInterval = value; } }
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

