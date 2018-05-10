using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class PulsingTween : MonoBehaviour 
{

    #region Variable Declarations
    [Space]
    [Range(0f, 1f)]
    [SerializeField] float maxAlpha = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] float minAlpha = 0.3f;
    [Range(0f, 2f)]
    [SerializeField] float scaleTo = 1.2f;
    [SerializeField] float duration = 2f;

    TextMeshProUGUI textMesh;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        textMesh = GetComponent<TextMeshProUGUI>();
        
        LeanTween.value(gameObject, minAlpha, maxAlpha, duration).setLoopPingPong().setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float value) => {
            Color newColor = textMesh.color;
            newColor.a = value;
            textMesh.color = newColor;
        });
        LeanTween.scale(gameObject.GetComponent<RectTransform>(), Vector3.one * scaleTo, duration).setLoopPingPong().setEase(LeanTweenType.easeInOutQuad);
	}
	#endregion
}
