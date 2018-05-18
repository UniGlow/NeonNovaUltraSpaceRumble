using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI spacingText;
    [SerializeField] TextMeshProUGUI artistText;

    Color targetColor;
    #endregion



    #region Unity Event Functions
    private void Start () 
	{
        targetColor = titleText.color;
	}
	
	private void Update () 
	{
		
	}
	#endregion
	
	
	
	#region Public Functions
	public void ShowSongTitle(string artist, string title)
    {
        // Alpha = 0
        Color newColor = targetColor;
        newColor.a = 0f;
        titleText.color = newColor;
        spacingText.color = newColor;
        artistText.color = newColor;

        // Set texts
        artistText.text = artist;
        titleText.text = title;
        spacingText.enabled = true;

        // Fade in alpha
        LeanTween.value(0f, targetColor.a, fadeDuration).setEaseInOutQuad().setOnUpdate((float value) =>
        {
            newColor.a = value;
            titleText.color = newColor;
            spacingText.color = newColor;
            artistText.color = newColor;
        }).setOnComplete(() => {
            LeanTween.value(targetColor.a, 0f, fadeDuration).setDelay(displayDuration).setEaseInOutQuad().setOnUpdate((float value) =>
            {
                newColor.a = value;
                titleText.color = newColor;
                spacingText.color = newColor;
                artistText.color = newColor;
            });
        });
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
}
