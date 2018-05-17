using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class HeroTutorialTextUpdater : MonoBehaviour 
{

    #region Variable Declarations
    [SerializeField] TextCollection textCollection;

    TextMeshPro textMesh;
    Hero hero;
    Quaternion startRotation;
    Vector3 startOffset;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        textMesh = GetComponent<TextMeshPro>();
        hero = transform.parent.GetComponent<Hero>();
        startRotation = transform.rotation;
        startOffset = transform.position - hero.transform.position;

        if (hero.ability == Ability.Damage) UpdateText("Damage");
        else if (hero.ability == Ability.Tank) UpdateText("Tank");
        else if (hero.ability == Ability.Opfer) UpdateText("Opfer");
    }
	
	private void Update () 
	{
        transform.position = hero.transform.position + startOffset;
        transform.rotation = startRotation;

        // Fade out text while moving to reduce clutter on screen
        //if (rigidbody.velocity.magnitude != 0f && !LeanTween.isTweening(tweenID) && textMesh.color.a != alphaWhileMoving)
        //{
        //    if (coroutine == null) coroutine = StartCoroutine(Wait(blendDelay, () =>
        //    {
        //        tweenID = LeanTween.value(gameObject, startColor.a, alphaWhileMoving, blendDuration).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float value) =>
        //        {
        //            Color newColor = startColor;
        //            newColor.a = value;
        //            textMesh.color = newColor;
        //        }).id;
        //    }));
        //}
        //else if (rigidbody.velocity.magnitude == 0f && !LeanTween.isTweening(tweenID) && textMesh.color.a != startColor.a)
        //{
        //    if (coroutine == null) coroutine = StartCoroutine(Wait(blendDelay, () =>
        //    {
        //        tweenID = LeanTween.value(gameObject, alphaWhileMoving, startColor.a, blendDuration).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float value) =>
        //        {
        //            Color newColor = startColor;
        //            newColor.a = value;
        //            textMesh.color = newColor;
        //        }).id;
        //    }));
        //}

        //newAlpha.a = Mathf.Lerp(alphaWhileMoving, startColor.a, Mathf.Max((10f - rigidbody.velocity.magnitude) / 10f, 0f));
    }
    #endregion



    #region Public Functions
    public void UpdateText(string title)
    {
        for (int i = 0; i < textCollection.messages.Length; i++)
        {
            if (textCollection.messages[i].title == title)
            {
                textMesh.text = textCollection.messages[i].body;
            }
        }
    }
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}
