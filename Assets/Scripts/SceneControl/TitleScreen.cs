using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class TitleScreen : MonoBehaviour
{

    #region Variable Declarations
    [Space]
    [SerializeField] int numberOfWords = 5;
    [SerializeField] float delayBetweenWords = 1f;
    [SerializeField] float fadeInTime = 0.6f;
    [SerializeField] float delayAfterCompletion = 3f;
    [SerializeField] bool screenshotMode;

    [Space]
    [SerializeField] List<TextMeshProUGUI> textFields;
    [SerializeField] List<string> words;
    [SerializeField] List<Color> colors;

    float timer;
    int currentWord;
    Vector3 targetScale;
    bool nextSceneLoaded;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        targetScale = textFields[0].transform.localScale;
        StartCoroutine(StartAudioNextFrame());
	}
	
	private void Update() {
        timer += Time.deltaTime;

        if (timer >= delayBetweenWords * delayAfterCompletion && currentWord >= numberOfWords && !nextSceneLoaded) {
            GameManager.Instance.LoadNextScene();
            nextSceneLoaded = true;
        }

        if (timer >= delayBetweenWords && currentWord < numberOfWords) {

            int rand;
            if (!screenshotMode) rand = Random.Range(0, words.Count);
            else rand = 0;

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
            if (!screenshotMode) textFields[currentWord].fontMaterial.SetColor("_GlowColor", colors[Random.Range(0,colors.Count)]);
            else textFields[currentWord].fontMaterial.SetColor("_GlowColor", colors[0]);
            colors.RemoveAt(rand);

            // Tween it in
            LeanTween.scale(textFields[currentWord].gameObject, targetScale, fadeInTime).setEase(LeanTweenType.easeOutBounce);
            currentWord++;
            timer = 0;
        }
	}
    #endregion



    #region Private Functions
    #endregion



    IEnumerator StartAudioNextFrame()
    {
        yield return null;
        AudioManager.Instance.StartTrack("OriginalTrack");
    }
}
