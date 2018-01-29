using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Switches the color of an image on the same GameObject recording to the defined color Gradient.
/// </summary>
[RequireComponent(typeof(Image))]
public class Rainbow : MonoBehaviour
{

    #region Variable Declarations
    [SerializeField] Gradient colors;
    [Tooltip("Time it takes to go trhough all colors of the defined color gradient.")]
    [SerializeField] float fadeTime = 2f;

    float timer;
    Image image;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        image = GetComponent<Image>();
	}
	
	private void Update() {
        timer += Time.deltaTime;

        image.color = colors.Evaluate((timer / fadeTime));

        if (timer >= fadeTime) timer = 0;
	}
	#endregion
	
	
	
	#region Private Functions
	#endregion
}
