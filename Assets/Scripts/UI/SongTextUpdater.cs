using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// 
/// </summary>
public class SongTextUpdater : MonoBehaviour 
{

    #region Variable Declarations
    [Space]
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float displayDuration = 7f;

    [Space]
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] TextMeshProUGUI spacingText = null;
    [SerializeField] TextMeshProUGUI artistText = null;

    Color originalColor;
    #endregion



    #region Unity Event Functions
    private void Awake () 
	{
        originalColor = titleText.color;
        HideText();
	}
	#endregion
	
	
	
	#region Public Functions
	public void ShowSongTitle(string artist, string title)
    {
        HideText();

        // Set texts
        artistText.text = artist;
        titleText.text = title;
        spacingText.text = "-";
        spacingText.enabled = true;

        // Fade in alpha
        DOTween.ToAlpha(() => titleText.color, x => titleText.color = x, originalColor.a, fadeDuration).OnComplete(() => { DOTween.ToAlpha(() => titleText.color, x => titleText.color = x, 0f, fadeDuration).SetDelay(displayDuration); });
        DOTween.ToAlpha(() => spacingText.color, x => spacingText.color = x, originalColor.a, fadeDuration).OnComplete(() => { DOTween.ToAlpha(() => spacingText.color, x => spacingText.color = x, 0f, fadeDuration).SetDelay(displayDuration); });
        DOTween.ToAlpha(() => artistText.color, x => artistText.color = x, originalColor.a, fadeDuration).OnComplete(() => { DOTween.ToAlpha(() => artistText.color, x => artistText.color = x, 0f, fadeDuration).SetDelay(displayDuration); });
    }
	#endregion
	
	
	
	#region Private Functions
	void HideText()
    {
        Color newColor = originalColor;
        newColor.a = 0f;
        titleText.color = newColor;
        spacingText.color = newColor;
        artistText.color = newColor;
    }
	#endregion
}
