using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// 
/// </summary>

[CreateAssetMenu(menuName = "Scriptable Objects/Player Config")]
public class PlayerConfig : ScriptableObject 
{

    #region Variable Declarations
    // Serialized Fields
    private Player player;
    private int playerNumber;
    private Faction faction;
    private PlayerColor colorConfig;
    //bleibt Public, da es sich ändern kann
    public Ability ability;
    private bool aiControlled;
    public Transform playerTransform;
    [SerializeField] HeroScore heroScore = null;

	// Private
	
	#endregion
	
	
	
	#region Public Properties
	public Player Player { get { return player; } }
    public int PlayerNumber { get { return playerNumber; } }
    public Faction Faction { get { return faction; } }
    public PlayerColor ColorConfig
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
    public HeroScore HeroScore { get { return heroScore; } }
	#endregion
	
	
	
	#region Public Functions
    /// <summary>
    /// Call this Method to Initialize this Players Config. It's not Recommended to do this outside the Lobby! Once set these Parameters can't be set without this Initialize-Method.
    /// </summary>
    /// <param name="player">Used to Identify this Player throughout all the Levels</param>
    /// <param name="faction">The Players Faction</param>
    /// <param name="colorConfig">The Players Color Configuration</param>
    /// <param name="aiControlled">Set this to True if the AI should control this Player</param>
	public void Initialize(Player player, int playerNumber, Faction faction, PlayerColor colorConfig, bool aiControlled)
    {
        this.player = player;
        this.playerNumber = playerNumber;
        this.faction = faction;
        this.colorConfig = colorConfig;
        this.aiControlled = aiControlled;
    }
	#endregion
	
	
	
	#region Private Functions

	#endregion
}

