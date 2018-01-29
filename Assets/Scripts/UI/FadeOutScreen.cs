using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class offers public functions to fade the screen in and out from and to an image.
/// </summary>
[RequireComponent(typeof(Image))]
public class FadeOutScreen : MonoBehaviour
{

    bool fading = false;


    /// <summary>
    /// Fades in the screen from the image on this GameObject.
    /// </summary>
    /// <param name="fadeInTime">Time in seconds the Fade-In shall take</param>
    public void FadeIn(float fadeInTime) {
        if (fading) {
            Debug.LogWarning("FadeIn couldn't start, because a fading is already in process.");
            return;
        }

        fading = true;
        RectTransform fadeOutImage = GetComponent<RectTransform>();

        LeanTween.alpha(fadeOutImage, 0f, fadeInTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
            fadeOutImage.GetComponent<Image>().enabled = false;
            fading = false;
        });
    }

    /// <summary>
    /// Fades in the screen from the image on this GameObject.
    /// </summary>
    /// <param name="fadeInTime">Time in seconds the Fade-In shall take</param>
    /// <param name="onComplete">Function to be called when the FadeIn is completed</param>
    public void FadeIn(float fadeInTime, System.Action onComplete) {
        if (fading) {
            Debug.LogWarning("FadeIn couldn't start, because a fading is already in process.");
            return;
        }

        fading = true;
        RectTransform fadeOutImage = GetComponent<RectTransform>();

        LeanTween.alpha(fadeOutImage, 0f, fadeInTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
            fadeOutImage.GetComponent<Image>().enabled = false;
            fading = false;
        }).setOnComplete(onComplete);
    }

    /// <summary>
    /// Fades the screen to the image on this GameObject.
    /// </summary>
    /// <param name="fadeOutTime">Time in seconds th Fade-Out shall take</param>
    public void FadeOut(float fadeOutTime) {
        if (fading) {
            Debug.LogWarning("FadeOut couldn't start, because a fading is already in process.");
            return;
        }

        fading = true;
        RectTransform fadeOutImage = GetComponent<RectTransform>();

        LeanTween.alpha(fadeOutImage, 1f, fadeOutTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
            fading = false;
        });
        fadeOutImage.GetComponent<Image>().enabled = true;
    }

    /// <summary>
    /// Fades the screen to the image on this GameObject.
    /// </summary>
    /// <param name="fadeOutTime">Time in seconds th Fade-Out shall take</param>
    /// <param name="onComplete">Function to be called when the FadeOut is completed</param>
    public void FadeOut(float fadeOutTime, System.Action onComplete) {
        if (fading) {
            Debug.LogWarning("FadeOut couldn't start, because a fading is already in process.");
            return;
        }

        fading = true;
        RectTransform fadeOutImage = GetComponent<RectTransform>();

        LeanTween.alpha(fadeOutImage, 1f, fadeOutTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
            fading = false;
        }).setOnComplete(onComplete);
        fadeOutImage.GetComponent<Image>().enabled = true;
    }
}
