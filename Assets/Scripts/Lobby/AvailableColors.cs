using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
//[CreateAssetMenu(menuName = "Scriptable Objects/Color Management/Available Colors")]
public class AvailableColors : ScriptableObject 
{

    #region Variable Declarations
    // Serialized Fields
    public List<PlayerColor> playerColors;
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions
	
	#endregion
	
	
	
	#region Public Functions
	public PlayerColor GetRandomColorExcept(PlayerColor color1 = null, PlayerColor color2 = null)
    {
        List<PlayerColor> colors = playerColors;
        if(color1 != null)
            colors.Remove(color1);
        if (color2 != null)
            colors.Remove(color2);
        return colors[Random.Range(0, colors.Count)];
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

