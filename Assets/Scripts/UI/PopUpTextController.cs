using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

/// <summary>
/// 
/// </summary>

public class PopUpTextController : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float maxSize = 1f;
    [SerializeField] private float popUpAnimationDuration = 0.7f;
    [SerializeField] private string bossWinText = "Boss wins!";
    [SerializeField] private string herosWinText = "Heros win!";

    // Private
    
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Awake()
    {
        text.gameObject.SetActive(false);
    }
    #endregion



    #region Public Functions
    public void WinTextPopUp(Faction faction)
    {
        if (DOTween.IsTweening(this))
            DOTween.Kill(this);
        text.gameObject.SetActive(true);
        text.transform.localScale = Vector3.zero;
        if (faction == Faction.Boss)
            text.text = bossWinText;
        else
            text.text = herosWinText;
        text.transform.DOScale(maxSize, popUpAnimationDuration).SetEase(Ease.OutBounce).SetUpdate(true).SetDelay(1f);
    }

    public void StartCountdown(float duration)
    {
        text.transform.localScale = Vector3.zero;
        text.gameObject.SetActive(true);
        text.text = "3";
        text.transform.DOScale(maxSize, duration/3f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() =>
        {
            text.text = "2";
            text.transform.localScale = Vector3.zero;
            text.transform.DOScale(maxSize, duration/3f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() =>
            {
                text.text = "1";
                text.transform.localScale = Vector3.zero;
                text.transform.DOScale(maxSize, duration/3f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() =>
                {
                    text.gameObject.SetActive(false);
                });
            });
        });
    }
	#endregion
	
	
	
	#region Private Functions

	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

