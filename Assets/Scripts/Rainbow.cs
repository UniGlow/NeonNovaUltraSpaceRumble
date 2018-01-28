using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class Rainbow : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] Gradient colors;
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
