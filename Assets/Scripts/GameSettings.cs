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
    [Tooltip("The point lead needed to win every gameplay level starts with.")]
    [SerializeField] private int pointLeadToWin = 500;

    [Space]
    [SerializeField] private float critDamageMultiplier = 2f;

    [Space]
    [SerializeField] private float intensifyTime = 24f;
    [Range(0, .9f)]
    [SerializeField] private float intensifyAmount = 0.1f;

    [Space]
    [SerializeField] private float bossColorSwitchInterval = 10f;


    // Private
    private ColorSet activeColorSet = null;
    
    #endregion



    #region Public Properties
    public ColorSet ActiveColorSet { get { return activeColorSet; } }
    public int PointLeadToWin { get { return pointLeadToWin; } }
    public float CritDamageMultiplier { get { return critDamageMultiplier; } set { critDamageMultiplier = value; } }
    public float IntensifyTime { get { return intensifyTime; } set { intensifyTime = value; } }
    public float IntensifyAmount { get { return intensifyAmount; } set { intensifyAmount = value; } }
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

