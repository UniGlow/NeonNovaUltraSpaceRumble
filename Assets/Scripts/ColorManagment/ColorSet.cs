using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

[CreateAssetMenu(menuName = "Scriptable Objects/Color Management/Color Set")]
public class ColorSet : ScriptableObject 
{

    #region Variable Declarations
    // Serialized Fields
    public PlayerColor2 color1;
    public PlayerColor2 color2;
    public PlayerColor2 color3;
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Public Functions
	public PlayerColor2 GetRandomColor()
    {
        int number = Random.Range(0, 3);
        switch (number)
        {
            case 0:
                return color1;
            case 1:
                return color2;
            case 2:
                return color3;
            default:
                return color1;
        }
    }

    public PlayerColor2 GetRandomColorExcept(PlayerColor2 playerColor)
    {
        List<PlayerColor2> colors = new List<PlayerColor2>();

        if (color1 != playerColor) colors.Add(color1);
        if (color2 != playerColor) colors.Add(color2);
        if (color3 != playerColor) colors.Add(color3);

        return colors[Random.Range(0, 2)];
    }
    #endregion



    #region Private Functions

    #endregion
}

