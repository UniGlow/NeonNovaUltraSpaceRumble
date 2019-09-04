using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

[CreateAssetMenu(menuName = "Scriptable Objects/Player Config")]
public class PlayerConfig : ScriptableObject 
{

    #region Variable Declarations
    // Serialized Fields
    private int playerNumber;
    private Faction faction;
    private PlayerColor2 colorConfig;
    //bleibt Public, da es sich ändern kann
    public Ability2 ability;
    private bool aiControlled;
    public Transform playerTransform;
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	public int PlayerNumber { get { return playerNumber; } }
    public Faction Faction { get { return faction; } }
    public PlayerColor2 ColorConfig
    {
        get
        {
            return colorConfig;
        }

        set
        {
            if (faction == Faction.Boss) colorConfig = value;
            else Debug.LogError("Tried to set colorConfig of a hero", this);
        }
    }
    public bool AIControlled { get { return aiControlled; } }
	#endregion
	
	
	
	#region Public Functions
    /// <summary>
    /// Call this Method to Initialize this Players Config. It's not Recommended to do this outside the Lobby! Once set these Parameters can't be set without this Initialize-Method.
    /// </summary>
    /// <param name="playerNumber">Used to Identify this Player throughout all the Levels</param>
    /// <param name="faction">The Players Faction</param>
    /// <param name="colorConfig">The Players Color Configuration</param>
    /// <param name="aiControlled">Set this to True if the AI should control this Player</param>
	public void Initialize(int playerNumber, Faction faction, PlayerColor2 colorConfig, bool aiControlled)
    {
        this.playerNumber = playerNumber;
        this.faction = faction;
        this.colorConfig = colorConfig;
        this.aiControlled = aiControlled;
    }
	#endregion
	
	
	
	#region Private Functions

	#endregion
}

