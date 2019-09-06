using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// </summary>
/// 
[CreateAssetMenu(menuName = "Scriptable Objects/Tutorial Screen")]
public class TutorialScreen : ScriptableObject 
{

    #region Variable Declarations
    // Serialized Fields
    [SerializeField] Sprite backgroundImage = null;
    [SerializeField] Sprite tutorialText = null;
    [Space]
    [SerializeField] TutorialScreen nextScreen = null;
    
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	public Sprite BackgroundImage { get { return backgroundImage; } }
    public Sprite TutorialText { get { return tutorialText; } }
    public TutorialScreen NextScreen { get { return nextScreen; } }
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

