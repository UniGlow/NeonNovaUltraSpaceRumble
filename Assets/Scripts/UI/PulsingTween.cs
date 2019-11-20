using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

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
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, minAlpha);
        DOTween.ToAlpha(() => textMesh.color, x => textMesh.color = x, maxAlpha, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        gameObject.GetComponent<RectTransform>().DOScale(Vector3.one * scaleTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
	}
    #endregion

    #region Public Functions
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
    #endregion
}
