using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class MatchNumberTextSetter : MonoBehaviour 
{

	#region Variable Declarations
	// Serialized Fields
	[SerializeField] TextMeshProUGUI text = null;
	// Private

	#endregion



	#region Public Properties

	#endregion



	#region Unity Event Functions

	#endregion



	#region Public Functions
	public void SetText(string text)
	{
		if (text == "∞")
		{
			this.text.fontStyle = FontStyles.Bold;
			this.text.alignment = TextAlignmentOptions.Left;
		}
		else
		{
			this.text.fontStyle = FontStyles.Normal;
			this.text.alignment = TextAlignmentOptions.TopLeft;
		}
		this.text.text = text;
	}
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

