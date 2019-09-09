using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GameEventRaiser : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    public GameEvent gameEvent;

	// Private
	
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions
	
	#endregion
	
	
	
	#region Public Functions
	public void RaiseGameEvent()
    {
        gameEvent.Raise(this);
    }
	#endregion
	
	
	
	#region Private Functions

	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}
