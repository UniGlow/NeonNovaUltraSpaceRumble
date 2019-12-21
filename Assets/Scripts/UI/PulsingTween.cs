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
    [Header("Text Alpha")]
    [Tooltip("If left empty, no alpha tweening will happen.")]
    [SerializeField] TextMeshProUGUI textMesh = null;
    [Range(0f, 1f)]
    [SerializeField] float maxAlpha = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] float minAlpha = 0.3f;

    [Header("Scaling")]
    [Tooltip("If left empty, no scaling will happen.")]
    [SerializeField] RectTransform rectTransform = null;
    [Range(0f, 2f)]
    [SerializeField] float scaleTo = 1.2f;
    [SerializeField] float duration = 2f;

    float originalAlpha;
    Vector3 originalScale;
    #endregion



    #region Unity Event Functions
    private void OnEnable () 
	{
        if (textMesh != null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            originalAlpha = textMesh.color.a;
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, minAlpha);
            DOTween.ToAlpha(() => textMesh.color, x => textMesh.color = x, maxAlpha, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetId(this).SetUpdate(true);
        }

        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
            rectTransform.DOScale(Vector3.one * scaleTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetId(this).SetUpdate(true);
        }
	}

    private void OnDisable()
    {
        DOTween.Kill(this);

        if (textMesh != null)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, originalAlpha);
        }

        if (rectTransform != null)
        {
            rectTransform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        }
    }
    #endregion

    #region Public Functions
    public void SetActive(bool value)
    {
        if (textMesh != null) textMesh.gameObject.SetActive(value);
        if (rectTransform != null) rectTransform.gameObject.SetActive(value);
    }

    /// <summary>
    /// Sets the active inverted.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <remarks>Specified "inverted" for use with GameEvents.</remarks>
    public void SetActiveInverted(bool value)
    {
        if (textMesh != null) textMesh.gameObject.SetActive(!value);
        if (rectTransform != null) rectTransform.gameObject.SetActive(!value);
    }
    #endregion
}
