using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class SelectionUI : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("References")]
    [Tooltip("The Number of this Panel")]
    [Range(1,4)]
    [SerializeField] int panelNumber = 1;
    [SerializeField] GameObject offlineScreen = null;
    [SerializeField] GameObject selectionScreen = null;
    [SerializeField] GameObject topPanel = null;
    [SerializeField] TextMeshProUGUI textMesh = null;
    [SerializeField] RawImage renderTexture = null;
    [SerializeField] Transform leftArrow = null;
    [SerializeField] Transform rightArrow = null;
    [SerializeField] Image backgroundImage = null;
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
		
	}
	#endregion
	
	
	
	#region Public Functions
	public void ChangeStep(int panelNumber, SelectionController.Step nextStep)
    {
        if (this.panelNumber == panelNumber)
        {
            switch (nextStep)
            {
                case SelectionController.Step.CharacterSelection:
                    offlineScreen.SetActive(false);
                    selectionScreen.SetActive(true);
                    break;
                case SelectionController.Step.Offline:
                    offlineScreen.SetActive(true);
                    selectionScreen.SetActive(false);
                    break;
            }
        }
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

