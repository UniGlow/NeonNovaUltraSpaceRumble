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
    [SerializeField]
    private List<PlayerColor> playerColors = new List<PlayerColor>();
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	public List<PlayerColor> PlayerColors { get { return playerColors; } }
	#endregion
	
	
	
	#region Unity Event Functions
	
	#endregion
	
	
	
	#region Public Functions
	public PlayerColor GetRandomColorExcept(PlayerColor color1 = null, PlayerColor color2 = null)
    {
        PlayerColor[] colors = new PlayerColor[playerColors.Count - (color1 == null ? (color2 == null ? 0 : 1) : (color2 == null ? 1 : 2))];
        int j = 0;
        for (int i=0; i<playerColors.Count; i++)
        {
            bool copy = false;
            if(color1 == null)
            {
                if (color2 == null)
                    copy = true;
                else if (color2 != playerColors[i])
                        copy = true;
            }
            else
            {
                if(color1 != playerColors[i])
                {
                    if (color2 == null)
                        copy = true;
                    else if (color2 != playerColors[i])
                            copy = true;
                }
            }
            if (copy)
            {
                colors[j] = playerColors[i];
                j++;
            }
        }
        return colors[Random.Range(0, colors.Length)];
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

