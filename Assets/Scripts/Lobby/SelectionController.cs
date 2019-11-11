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
    [SerializeField] List<Transform> models = new List<Transform>();
    [SerializeField] GameEvent playerChangedStepEvent = null;


    // Private
    bool inputsLocked = false;
    Rewired.Player player = null;
    Step activeStep = Step.Offline;
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void Start()
    {
        InputHelper.ChangeRuleSetForAllPlayers(RewiredConsts.LayoutManagerRuleSet.RULESETLOBBY);
    }

    private void Update()
    {
        ManageReadyInputs();
        if(player != null && !inputsLocked)
        {
            ManageSelection();
        }
    }
    #endregion



    #region Public Functions

    #endregion



    #region Private Functions
    void ManageReadyInputs()
    {
        if (player == null)
        {
            Rewired.Player player = InputHelper.GetPlayerButtonDown(RewiredConsts.Action.UISUBMIT);
            if (player != null)
            {
                Debug.Log("Player Joined: " + player.name);
                this.player = player;
                activeStep = Step.CharacterSelection;
                RaisePlayerChangedStep(panelNumber, Step.CharacterSelection);
            }
        }
        else
        {
            if (player.GetButtonDown(RewiredConsts.Action.UICANCEL))
            {
                switch (activeStep)
                {
                    case Step.Offline:
                        // No Player should be able to access this Step! Check Player Variable, it should be null!
                        Debug.LogWarning("Something went wrong here! A player pressed B but shouldn't be able to!");
                        break;
                    case Step.CharacterSelection:
                        RaisePlayerChangedStep(panelNumber, Step.Offline);
                        activeStep = Step.Offline;
                        player = null;
                        break;
                    case Step.ColorSelection:
                        RaisePlayerChangedStep(panelNumber, Step.CharacterSelection);
                        activeStep = Step.CharacterSelection;
                        break;
                    case Step.AbilitySelection:
                        RaisePlayerChangedStep(panelNumber, Step.ColorSelection);
                        activeStep = Step.ColorSelection;
                        break;
                    case Step.ReadyToPlay:
                        // TODO: Uncomment following Lines once Abilities should be selectable in Lobby!
                        //RaisePlayerChangedStepEvent(panelNumber, Step.AbilitySelection);
                        //activeStep = Step.AbilitySelection;
                        RaisePlayerChangedStep(panelNumber, Step.ColorSelection);
                        activeStep = Step.ColorSelection;
                        break;
                    default:
                        Debug.LogWarning("Something went wrong here! Eather a new Step didn't get implemented or some Error accured!");
                        break;
                }
            }
            else if(player.GetButtonDown(RewiredConsts.Action.UISUBMIT))
            {
                switch (activeStep)
                {
                    case Step.Offline:
                        // Player Variable was set but activeStep didn't get set to CharacterSelection! Check activeStep Variable, it shouldn't be Offline!
                        Debug.LogWarning("Something went wrong here!");
                        break;
                    case Step.CharacterSelection:
                        RaisePlayerChangedStep(panelNumber, Step.ColorSelection);
                        activeStep = Step.ColorSelection;
                        break;
                    case Step.ColorSelection:
                        // TODO: Uncomment following Lines once Abilities should be selectable in Lobby!
                        //RaisePlayerChangedStepEvent(panelNumber, Step.AbilitySelection);
                        //activeStep = Step.AbilitySelection;
                        RaisePlayerChangedStep(panelNumber, Step.ReadyToPlay);
                        activeStep = Step.ReadyToPlay;
                        break;
                    case Step.ReadyToPlay:
                        // This step doesn't do anything, the player is just nervously pressing the A button :)
                        break;
                    default:
                        Debug.LogWarning("Something went wrong here! Eather a new Step didn't get implemented or some Error accured!");
                        break;
                }
            }
        }
    }

    void ManageSelection()
    {
        // TODO: We should have configurable Deadzones for all Controllers! Eather Rewired does this automaticly and ajusts
        // the Inputs accordingly or we should write a GameSetting for this and implement it here
        if(player.GetAxis(RewiredConsts.Action.UIHORIZONTAL) > 0)
        {
            //Debug.Log("UIHorizontal: " + player.GetAxis(RewiredConsts.Action.UIHORIZONTAL));
            inputsLocked = true;
            Vector3 targetRotation = new Vector3(wheel.transform.rotation.eulerAngles.x, wheel.transform.rotation.eulerAngles.y + 90, wheel.transform.rotation.eulerAngles.z);
            //Debug.Log("Old Rotation: " + wheel.transform.rotation + " | New Rotation: " + targetRotation);
            wheel.DORotate(targetRotation, rotationDuration).SetEase(animationCurve).OnComplete(() =>
                {
                    inputsLocked = false;
                }
            );
        }
        else if(player.GetAxis(RewiredConsts.Action.UIHORIZONTAL) < 0)
        {
            //Debug.Log("UIHorizontal: " + player.GetAxis(RewiredConsts.Action.UIHORIZONTAL));
            inputsLocked = true;
            Vector3 targetRotation = new Vector3(wheel.transform.rotation.eulerAngles.x, wheel.transform.rotation.eulerAngles.y - 90, wheel.transform.rotation.eulerAngles.z);
            //Debug.Log("Old Rotation: " + wheel.transform.rotation +  " | New Rotation: " + targetRotation);
            wheel.DORotate(targetRotation, rotationDuration).SetEase(animationCurve).OnComplete(() =>
            {
                inputsLocked = false;
            }
            );
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
    #endregion



    #region Coroutines

    #endregion
}

