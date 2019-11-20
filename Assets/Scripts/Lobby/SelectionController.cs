using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 
/// </summary>
public class SelectionController : MonoBehaviour 
{
    public enum Step
    {
        Offline,
        CharacterSelection,
        ColorSelection,
        AbilitySelection,
        ReadyToPlay
    }
    #region Variable Declarations
    // Serialized Fields
    [Header("References")]
    [Tooltip("The Panel this Controller should Control")]
    [Range(1,4)]
    [SerializeField] int panelNumber = 1;
    [SerializeField] float rotationDuration = 1f;
    [SerializeField] AnimationCurve animationCurve = new AnimationCurve();
    [SerializeField] Transform wheel = null;
    [SerializeField] List<Transform> heroModels = new List<Transform>();
    [SerializeField] GameEvent playerChangedStepEvent = null;
    [SerializeField] GameEvent playerChangedColor = null;
    [SerializeField] float delayBetweenChanges = 0.25f;


    // Private
    bool listeningForPlayerInput = false;
    bool inputsLocked = false;
    Rewired.Player player = null;
    Step activeStep = Step.Offline;
    bool isBoss = true; // This will be used, to show only the relevant Steps to the Player - Example: Boss cannot choose Color
    PlayerColor activeColor = null;
    float changeTimer = 0f;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Update()
    {
        ManageReadyInputs();
        if (player != null && !inputsLocked)
        {
            ManageSelection();
        }
        if(changeTimer != 0f)
        {
            changeTimer += Time.deltaTime;
        }
        if(changeTimer >= delayBetweenChanges)
        {
            changeTimer = 0f;
        }
    }
    #endregion



    #region Public Functions
    public void ListeningForInputsChanged(bool[] playerListeningForInput)
    {
        listeningForPlayerInput = playerListeningForInput[panelNumber - 1];
    }

    public void UpdateModelColor(int panelNumber, PlayerColor newColor)
    {
        if (panelNumber == this.panelNumber)
        {
            activeColor = newColor;
            foreach (Transform model in heroModels)
            {
                model.GetComponent<MeshRenderer>().material = newColor.heroMaterial;
            }
        }
    }
    #endregion



    #region Private Functions
    void ManageReadyInputs()
    {
        if (player == null && listeningForPlayerInput)
        {
            Rewired.Player tempPlayer = InputHelper.GetPlayerButtonDown(RewiredConsts.Action.UISUBMIT);
            if (tempPlayer != null && !NewSirAlfredLobby.Instance.IsPlayerActive(tempPlayer))
            {
                Debug.Log("Player Joined: " + tempPlayer.name);
                this.player = tempPlayer;
                NewSirAlfredLobby.Instance.SetPlayer(panelNumber, tempPlayer);
                activeStep = Step.CharacterSelection;
                Debug.Log("Raising Event");
                RaisePlayerChangedStep(panelNumber, activeStep);
            }
        }
        else if(player != null)
        {
            bool playerPressedButton = false;
            // Player pressed B
            if (player.GetButtonDown(RewiredConsts.Action.UICANCEL))
            {
                playerPressedButton = true;
                switch (activeStep)
                {
                    case Step.Offline:
                        // No Player should be able to access this Step! Check Player Variable, it should be null!
                        Debug.LogWarning("Something went wrong here! A player pressed B but shouldn't be able to!");
                        break;
                    case Step.CharacterSelection:
                        RaisePlayerChangedStep(panelNumber, Step.Offline);
                        activeStep = Step.Offline;
                        break;
                    case Step.ColorSelection:
                        RaisePlayerChangedStep(panelNumber, Step.CharacterSelection);
                        activeStep = Step.CharacterSelection;
                        break;
                    case Step.AbilitySelection:
                        if (isBoss)
                        {
                            RaisePlayerChangedStep(panelNumber, Step.CharacterSelection);
                            activeStep = Step.CharacterSelection;
                        }
                        else
                        {
                            NewSirAlfredLobby.Instance.SetPlayerColor(panelNumber, null);
                            RaisePlayerChangedStep(panelNumber, Step.ColorSelection);
                            activeStep = Step.ColorSelection;
                        }
                        break;
                    case Step.ReadyToPlay:
                        // TODO: Uncomment following Lines once Abilities should be selectable in Lobby! Delete the rest
                        //RaisePlayerChangedStepEvent(panelNumber, Step.AbilitySelection);
                        //activeStep = Step.AbilitySelection;
                        if (isBoss)
                        {
                            RaisePlayerChangedStep(panelNumber, Step.CharacterSelection);
                            activeStep = Step.CharacterSelection;
                        }
                        else
                        {
                            NewSirAlfredLobby.Instance.SetPlayerColor(panelNumber, null);
                            RaisePlayerChangedStep(panelNumber, Step.ColorSelection);
                            activeStep = Step.ColorSelection;
                        }
                        // But don't delete this
                        NewSirAlfredLobby.Instance.SetReadyToPlay(panelNumber, false);
                        break;
                    default:
                        Debug.LogWarning("Something went wrong here! Eather a new Step didn't get implemented or some Error accured!");
                        break;
                }
            }
            // Player Pressed A
            else if(player.GetButtonDown(RewiredConsts.Action.UISUBMIT))
            {
                playerPressedButton = true;
                switch (activeStep)
                {
                    case Step.Offline:
                        // Player Variable was set but activeStep didn't get set to CharacterSelection! Check activeStep Variable, it shouldn't be Offline!
                        Debug.LogWarning("Something went wrong here!");
                        break;
                    case Step.CharacterSelection:
                        if (isBoss)
                        {
                            RaisePlayerChangedStep(panelNumber, Step.ReadyToPlay);
                            activeStep = Step.ReadyToPlay;
                        }
                        else
                        {
                            RaisePlayerChangedStep(panelNumber, Step.ColorSelection);
                            activeStep = Step.ColorSelection;
                        }
                        break;
                    case Step.ColorSelection:
                        // TODO: Uncomment following Lines once Abilities should be selectable in Lobby! Delete the Rest.
                        //RaisePlayerChangedStepEvent(panelNumber, Step.AbilitySelection);
                        //activeStep = Step.AbilitySelection;
                        RaisePlayerChangedStep(panelNumber, Step.ReadyToPlay);
                        activeStep = Step.ReadyToPlay;
                        // Don't delete this
                        NewSirAlfredLobby.Instance.SetPlayerColor(panelNumber, activeColor);
                        break;
                    case Step.ReadyToPlay:
                        // This step doesn't do anything, the player is just nervously pressing the A button :)
                        // Ignoring that Input so Alfred doesn't have to deal with it :D
                        playerPressedButton = false;
                        break;
                    default:
                        Debug.LogWarning("Something went wrong here! Eather a new Step didn't get implemented or some Error accured!");
                        break;
                }
            }
            // Following Code has been extracted from aboth for better usability. This happens once a specific Step has been entered
            if (playerPressedButton)
            {
                switch (activeStep)
                {
                    case Step.Offline:
                        NewSirAlfredLobby.Instance.SetPlayer(panelNumber, null);
                        player = null;
                        break;
                    case Step.ColorSelection:
                        if(activeColor == null)
                            activeColor = NewSirAlfredLobby.Instance.AvailableColors[0];
                        RaisePlayerChangedColor(panelNumber, activeColor);
                        break;
                    case Step.ReadyToPlay:
                        NewSirAlfredLobby.Instance.SetReadyToPlay(panelNumber, true);
                        break;
                }
            }
        }
    }

    void ManageSelection()
    {
        if(player.GetAxis(RewiredConsts.Action.UIHORIZONTAL) > 0)
        {
            switch (activeStep)
            {
                case Step.CharacterSelection:
                    //Debug.Log("UIHorizontal: " + player.GetAxis(RewiredConsts.Action.UIHORIZONTAL));
                    inputsLocked = true;
                    Vector3 targetRotation = new Vector3(wheel.transform.rotation.eulerAngles.x, wheel.transform.rotation.eulerAngles.y + 90, wheel.transform.rotation.eulerAngles.z);
                    isBoss = NewSirAlfredLobby.Instance.SwitchPlayerCharacter(panelNumber, NewSirAlfredLobby.Direction.Right);
                    //Debug.Log("Old Rotation: " + wheel.transform.rotation + " | New Rotation: " + targetRotation);
                    wheel.DORotate(targetRotation, rotationDuration).SetEase(animationCurve).OnComplete(() =>
                    {
                        inputsLocked = false;
                    });
                    break;
                case Step.ColorSelection:
                    if (changeTimer == 0f)
                    {
                        PlayerColor tempColor = NewSirAlfredLobby.Instance.GetNextAvailableColor(true, activeColor);
                        if (activeColor != tempColor)
                        {
                            activeColor = tempColor;
                            RaisePlayerChangedColor(panelNumber, activeColor);
                        }
                        changeTimer += Time.deltaTime;
                    }
                    break;
            }
        }
        else if(player.GetAxis(RewiredConsts.Action.UIHORIZONTAL) < 0)
        {
            switch (activeStep)
            {
                case Step.CharacterSelection:
                    //Debug.Log("UIHorizontal: " + player.GetAxis(RewiredConsts.Action.UIHORIZONTAL));
                    inputsLocked = true;
                    Vector3 targetRotation = new Vector3(wheel.transform.rotation.eulerAngles.x, wheel.transform.rotation.eulerAngles.y - 90, wheel.transform.rotation.eulerAngles.z);
                    isBoss = NewSirAlfredLobby.Instance.SwitchPlayerCharacter(panelNumber, NewSirAlfredLobby.Direction.Left);
                    //Debug.Log("Old Rotation: " + wheel.transform.rotation +  " | New Rotation: " + targetRotation);
                    wheel.DORotate(targetRotation, rotationDuration).SetEase(animationCurve).OnComplete(() =>
                    {
                        inputsLocked = false;
                    });
                    break;
                case Step.ColorSelection:
                    if (changeTimer == 0f)
                    {
                        PlayerColor tempColor = NewSirAlfredLobby.Instance.GetNextAvailableColor(false, activeColor);
                        if (activeColor != tempColor)
                        {
                            activeColor = tempColor;
                            RaisePlayerChangedColor(panelNumber, activeColor);
                        }
                        changeTimer += Time.deltaTime;
                    }
                    break;
            }
        }
    }

    void ManageCharacterRotation()
    {
        
    }
    #endregion



    #region GameEvent Raiser
    void RaisePlayerChangedStep(int panelNumber, Step newStep)
    {
        playerChangedStepEvent.Raise(this, panelNumber, newStep);
    }

    void RaisePlayerChangedColor(int panelNumber, PlayerColor color)
    {
        playerChangedColor.Raise(this, panelNumber, color);
    }
    #endregion



    #region Coroutines

    #endregion
}