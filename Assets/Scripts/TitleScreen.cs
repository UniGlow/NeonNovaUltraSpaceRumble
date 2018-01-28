using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class TitleScreen : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] int numberOfWords = 5;
    [SerializeField] float delayBetweenWords = 1f;
    [SerializeField] float fadeInTime = 0.6f;

    [Space]
    [SerializeField] List<TextMeshProUGUI> textFields;
    [SerializeField] List<string> words;
    [SerializeField] List<Color> colors;

    float timer;
    int currentWord;
    Vector3 targetScale;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        targetScale = textFields[0].transform.localScale;
	}
	
	private void Update() {
        if (currentWord < numberOfWords) {
            timer += Time.deltaTime;

            if (timer >= delayBetweenWords) {

                int rand = Random.Range(0, words.Count);

                // Ultra mustn't stand at the last spot
                if (currentWord == numberOfWords - 2 && words.Contains("Ultra")) {
                    rand = words.FindIndex(x => x == "Ultra");
                }

                // Activate textField
                textFields[currentWord].transform.localScale = Vector3.zero;
                textFields[currentWord].gameObject.SetActive(true);

                // Set random word
                textFields[currentWord].text = words[rand];
                words.RemoveAt(rand);
                
                // Set random color
                textFields[currentWord].fontMaterial.SetColor("_OutlineColor", colors[rand]);
                textFields[currentWord].fontMaterial.SetColor("_GlowColor", colors[Random.Range(0,colors.Count)]);
                colors.RemoveAt(rand);

                // Tween it in
                LeanTween.scale(textFields[currentWord].gameObject, targetScale, fadeInTime).setEase(LeanTweenType.easeOutBounce);
                currentWord++;
                timer = 0;
            }
        }
	}
	#endregion
	
	
	
	#region Private Functions
	#endregion
}
