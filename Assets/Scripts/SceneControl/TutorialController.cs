using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Rewired;

/// <summary>
/// 
/// </summary>
public class TutorialController : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("References")]
    [SerializeField] TutorialScreen firstScreen = null;
    [SerializeField] Image backgroundImage = null;
    [SerializeField] Image tutorialText = null;
    [SerializeField] Image player1ReadyImage = null;
    [SerializeField] Image player2ReadyImage = null;
    [SerializeField] Image player3ReadyImage = null;
    [SerializeField] Image player4ReadyImage = null;
    [Space]
    [SerializeField] GameObject player1ReadyPanel = null;
    [SerializeField] GameObject player2ReadyPanel = null;
    [SerializeField] GameObject player3ReadyPanel = null;
    [SerializeField] GameObject player4ReadyPanel = null;
    [SerializeField] GameObject loadingLobbyField = null;

    [Header("Resources")]
    [SerializeField] Sprite playerNotReadyImage = null;
    [SerializeField] Sprite playerReadyImage = null;

    [Header("Properties")]
    [SerializeField] float fadeTimeBetweenScreens = 1f;
    [SerializeField] float minAlphaPlayerReadyImages = .3f;
    [SerializeField] float minSizePlayerReadyImages = .5f;
    [SerializeField] float readyPlayerImageAnimationCycleDuration = 2f;
    // Private
    private TutorialScreen activeScreen;
    private int playerCount;
    private bool player1Ready = false;
    private bool player2Ready = false;
    private bool player3Ready = false;
    private bool player4Ready = false;
    private bool tweening = false;
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        Time.timeScale = 1f;
        loadingLobbyField.SetActive(false);

        activeScreen = firstScreen;
        backgroundImage.sprite = activeScreen.BackgroundImage;
        tutorialText.sprite = activeScreen.TutorialText;
        player1ReadyImage.sprite = playerNotReadyImage;
        player2ReadyImage.sprite = playerNotReadyImage;
        player3ReadyImage.sprite = playerNotReadyImage;
        player4ReadyImage.sprite = playerNotReadyImage;

        // Infinite Loop Animate the Ready-Images
        player1ReadyImage.DOFade(minAlphaPlayerReadyImages, readyPlayerImageAnimationCycleDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        player1ReadyImage.transform.DOScale(minSizePlayerReadyImages, readyPlayerImageAnimationCycleDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        player2ReadyImage.DOFade(minAlphaPlayerReadyImages, readyPlayerImageAnimationCycleDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        player2ReadyImage.transform.DOScale(minSizePlayerReadyImages, readyPlayerImageAnimationCycleDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        player3ReadyImage.DOFade(minAlphaPlayerReadyImages, readyPlayerImageAnimationCycleDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        player3ReadyImage.transform.DOScale(minSizePlayerReadyImages, readyPlayerImageAnimationCycleDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        player4ReadyImage.DOFade(minAlphaPlayerReadyImages, readyPlayerImageAnimationCycleDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        player4ReadyImage.transform.DOScale(minSizePlayerReadyImages, readyPlayerImageAnimationCycleDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        if (!tweening)
        {
            playerCount = InputHelper.UpdatePlayerCount();
            UpdateReadyImagesAmount();
            CheckControllerInput();
            if (CheckAllPlayerReady())
            {
                GoToNextScreen();
            }
        }
    }
    #endregion



    #region Public Functions

    #endregion



    #region Private Functions
    private void UpdateReadyImagesAmount()
    {
        switch (playerCount)
        {
            case 0:
                player1ReadyPanel.SetActive(true);
                player2ReadyPanel.SetActive(false);
                player3ReadyPanel.SetActive(false);
                player4ReadyPanel.SetActive(false);
                break;
            case 1:
                player1ReadyPanel.SetActive(true);
                player2ReadyPanel.SetActive(false);
                player3ReadyPanel.SetActive(false);
                player4ReadyPanel.SetActive(false);
                break;
            case 2:
                player1ReadyPanel.SetActive(true);
                player2ReadyPanel.SetActive(true);
                player3ReadyPanel.SetActive(false);
                player4ReadyPanel.SetActive(false);
                break;
            case 3:
                player1ReadyPanel.SetActive(true);
                player2ReadyPanel.SetActive(true);
                player3ReadyPanel.SetActive(true);
                player4ReadyPanel.SetActive(false);
                break;
            case 4:
                player1ReadyPanel.SetActive(true);
                player2ReadyPanel.SetActive(true);
                player3ReadyPanel.SetActive(true);
                player4ReadyPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void CheckControllerInput()
    {
        if (playerCount == 0)
        {
            // Only for debugging purposes, to be able to cycle through the screens when no player is registered
            if (Input.GetKeyDown(KeyCode.Return))
            {
                player1ReadyImage.sprite = playerReadyImage;
                player1Ready = true;
            }
        }
        if (playerCount >= 1)
        {
            if(ReInput.players.Players[0].GetButtonDown(RewiredConsts.Action.READY_UP) && player1ReadyImage != playerReadyImage)
            {
                player1ReadyImage.sprite = playerReadyImage;
                player1Ready = true;
            }
            else if(ReInput.players.Players[0].GetButtonDown(RewiredConsts.Action.UICANCEL) && player1ReadyImage != playerNotReadyImage)
            {
                player1ReadyImage.sprite = playerNotReadyImage;
                player1Ready = false;
            }
        }
        if (playerCount >= 2)
        {
            if (ReInput.players.Players[1].GetButtonDown(RewiredConsts.Action.READY_UP) && player2ReadyImage != playerReadyImage)
            {
                player2ReadyImage.sprite = playerReadyImage;
                player2Ready = true;
            }
            else if (ReInput.players.Players[1].GetButtonDown(RewiredConsts.Action.UICANCEL) && player2ReadyImage != playerNotReadyImage)
            {
                player2ReadyImage.sprite = playerNotReadyImage;
                player2Ready = false;
            }
        }
        if (playerCount >= 3)
        {
            if (ReInput.players.Players[2].GetButtonDown(RewiredConsts.Action.READY_UP) && player3ReadyImage != playerReadyImage)
            {
                player3ReadyImage.sprite = playerReadyImage;
                player3Ready = true;
            }
            else if (ReInput.players.Players[2].GetButtonDown(RewiredConsts.Action.UICANCEL) && player3ReadyImage != playerNotReadyImage)
            {
                player3ReadyImage.sprite = playerNotReadyImage;
                player3Ready = false;
            }
        }
        if (playerCount == 4)
        {
            if (ReInput.players.Players[3].GetButtonDown(RewiredConsts.Action.READY_UP) && player4ReadyImage != playerReadyImage)
            {
                player4ReadyImage.sprite = playerReadyImage;
                player4Ready = true;
            }
            else if (ReInput.players.Players[3].GetButtonDown(RewiredConsts.Action.UICANCEL) && player4ReadyImage != playerNotReadyImage)
            {
                player4ReadyImage.sprite = playerNotReadyImage;
                player4Ready = false;
            }
        }
    }

    private bool CheckAllPlayerReady()
    {
        switch (playerCount)
        {
            case 0:
                if (player1Ready)
                    return true;
                else return false;
            case 1:
                if (player1Ready)
                    return true;
                else
                    return false;
            case 2:
                if (player1Ready && player2Ready)
                    return true;
                else
                    return false;
            case 3:
                if (player1Ready && player2Ready && player3Ready)
                    return true;
                else
                    return false;
            case 4:
                if (player1Ready && player2Ready && player3Ready && player4Ready)
                    return true;
                else
                    return false;
            default:
                break;
        }
        return false;
    }

    private void GoToNextScreen()
    {
        if(activeScreen.NextScreen != null)
        {
            tweening = true;
            activeScreen = activeScreen.NextScreen;
            backgroundImage.DOFade(0f, fadeTimeBetweenScreens).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                backgroundImage.sprite = activeScreen.BackgroundImage;
                backgroundImage.DOFade(1f, fadeTimeBetweenScreens).SetEase(Ease.InOutQuad);

                player1ReadyImage.sprite = playerNotReadyImage;
                player1Ready = false;
                player2ReadyImage.sprite = playerNotReadyImage;
                player2Ready = false;
                player3ReadyImage.sprite = playerNotReadyImage;
                player3Ready = false;
                player4ReadyImage.sprite = playerNotReadyImage;
                player4Ready = false;
                tweening = false;
            });
            tutorialText.DOFade(0f, fadeTimeBetweenScreens).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                tutorialText.sprite = activeScreen.TutorialText;
                tutorialText.DOFade(1f, fadeTimeBetweenScreens).SetEase(Ease.InOutQuad);
            });
        }
        else
        {
            loadingLobbyField.SetActive(true);
            SceneManager.Instance.LoadLobby();
        }
    }
    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}

