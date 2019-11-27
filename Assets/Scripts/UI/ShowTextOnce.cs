using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// 
/// </summary>
public class ShowTextOnce : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [SerializeField] TextMeshProUGUI text = null;
    [SerializeField] float duration = 5f;
    [SerializeField] float maxAlpha = 1f;
    [SerializeField] float transitionInDuration = 0.5f;
    [SerializeField] float transitionOutDuration = 0.5f;
	// Private
	
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions

	#endregion
	
	
	
	#region Public Functions
	public void ShowText()
    {
        text.alpha = 0f;
        text.gameObject.SetActive(true);
        text.DOFade(maxAlpha, transitionInDuration).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            text.DOFade(0f, transitionOutDuration).SetEase(Ease.InOutCubic).SetDelay(duration);
        });
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

