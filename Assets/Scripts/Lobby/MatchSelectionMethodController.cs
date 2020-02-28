using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class MatchSelectionMethodController : MonoBehaviour 
{

	#region Variable Declarations
	// Serialized Fields
	[SerializeField] GameObject[] matchNumberObjects = null;
	[SerializeField] GameObject[] bestOfObjects = null;
	[SerializeField] GameObject ReadyToStartObject = null;
	[SerializeField] GameSettings gameSettings = null;
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions
	
	#endregion
	
	
	
	#region Public Functions
	public void ReadyToStart(bool state)
	{
		// Show only appropriate UI
		if (gameSettings.UseBestOfFeature)
		{
			foreach(GameObject go in bestOfObjects)
			{
				go.SetActive(true);
			}
			foreach(GameObject go in matchNumberObjects)
			{
				go.SetActive(false);
			}
		}
		else
		{
			foreach (GameObject go in bestOfObjects)
			{
				go.SetActive(false);
			}
			foreach (GameObject go in matchNumberObjects)
			{
				go.SetActive(true);
			}
		}
		ReadyToStartObject.SetActive(state);
	}
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

