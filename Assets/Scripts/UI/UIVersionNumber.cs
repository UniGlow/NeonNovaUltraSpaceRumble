using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class UIVersionNumber : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [SerializeField] TextMeshProUGUI releaseLabel = null;
    [SerializeField] TextMeshProUGUI versionNumberLabel = null;
    [SerializeField] VersionNumber versionNumberObject = null;
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        releaseLabel.text = versionNumberObject.releaseText;
        versionNumberLabel.text = versionNumberObject.versionNumber;
	}
	#endregion
	
	
	
	#region Public Functions
	
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

