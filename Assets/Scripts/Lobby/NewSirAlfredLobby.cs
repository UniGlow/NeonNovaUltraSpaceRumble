using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class NewSirAlfredLobby : MonoBehaviour 
{

    #region Variable Declarations
    public static NewSirAlfredLobby Instance = null;
    // Serialized Fields
    [SerializeField] GameEvent slotListeningForInputEvent = null;
    [Tooltip("The Scriptable Object that holds all available Colors in the Game")]
    [SerializeField] AvailableColors availableColors = null;
    [Tooltip("The Scriptable Object that will be used during the battles which only holds 3 Colors")]
    [SerializeField] ColorSet activeColorSet = null;
    // Private
    // Player Settings
    Rewired.Player[] players = new Rewired.Player[4] { null, null, null, null };
    bool[] playerListeningForInput = new bool[4] { true, false, false, false };
    bool[] playersActive = new bool[4] { false, false, false, false };
    PlayerColor[] playerColors = new PlayerColor[4] { null, null, null, null };

    // Data
    List<PlayerColor> availablePlayerColors = new List<PlayerColor>();
    #endregion



    #region Public Properties
    public List<PlayerColor> AvailableColors { get { return availablePlayerColors; } }
    #endregion



    #region Unity Event Functions
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InputHelper.ChangeRuleSetForAllPlayers(RewiredConsts.LayoutManagerRuleSet.RULESETLOBBY);
        RaiseSlotsListeningForInputs(playerListeningForInput);
        availablePlayerColors = availableColors.playerColors;
    }
    #endregion



    #region Public Functions
    public void PlayerChangedState(int panelNumber, SelectionController.Step activeStep)
    {
        bool somethingChanged = false;
        if(activeStep == SelectionController.Step.Offline)
        {
            playerListeningForInput[panelNumber - 1] = true;
            playersActive[panelNumber - 1] = false;
            bool firstElementChanged = false;
            for(int i = 0; i < playerListeningForInput.Length; i++)
            {
                if (!firstElementChanged)
                {
                    if (playerListeningForInput[i] == true)
                    {
                        firstElementChanged = true;
                    }
                }
                else
                {
                    playerListeningForInput[i] = false;
                }
            }
            somethingChanged = true;
        }
        else if(activeStep == SelectionController.Step.CharacterSelection)
        {
            playerListeningForInput[panelNumber - 1] = false;
            playersActive[panelNumber - 1] = true;
            bool firstElementChanged = false;
            for (int i = 0; i < playerListeningForInput.Length; i++)
            {
                if (!firstElementChanged)
                {
                    if (!playerListeningForInput[i] && !playersActive[i])
                    {
                        playerListeningForInput[i] = true;
                        firstElementChanged = true;
                    }
                }
                else
                {
                    playerListeningForInput[i] = false;
                }
            }
            somethingChanged = true;
        }
        if (somethingChanged)
        {
            string log = "Slots listening for Input:\n";
            for (int i = 0; i < playerListeningForInput.Length; i++)
                log += "P" + (i+1) + ": " + playerListeningForInput[i] + "\n";
            Debug.Log(log);
            RaiseSlotsListeningForInputs(playerListeningForInput);
        }
    }

    public void SetPlayer(int panelNumber, Rewired.Player player)
    {
        players[panelNumber - 1] = player;
    }

    public bool IsPlayerActive(Rewired.Player player)
    {
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] == player)
                return true;
        }
        return false;
    }

    public void SetPlayerColor(int panelNumber, PlayerColor playerColor)
    {
        playerColors[panelNumber - 1] = playerColor;
        if (playerColor != null)
        {
            availablePlayerColors.Remove(playerColor);
        }
        else
        {
            availablePlayerColors.Add(playerColor);
        }
    }
    /// <summary>
    /// Returns next Available Color when next is true
    /// Return previous Available Color when next is false
    /// </summary>
    /// <param name="next">true = next Color | false = previous Color</param>
    /// <param name="activeColor"></param>
    /// <returns></returns>
    public PlayerColor GetNextAvailableColor(bool next, PlayerColor activeColor)
    {
        int index = availablePlayerColors.IndexOf(activeColor);
        if (next)
        {
            if (index != availablePlayerColors.Count - 1)
            {
                return availablePlayerColors[index + 1];
            }
            else
            {
                return availablePlayerColors[0];
            }
        }
        else
        {
            if(index != 0)
            {
                return availablePlayerColors[index - 1];
            }
            else
            {
                return availablePlayerColors[availablePlayerColors.Count - 1];
            }
        }
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
	
	
	
	#region GameEvent Raiser
	void RaiseSlotsListeningForInputs(bool[] playerListeningForInput)
    {
        slotListeningForInputEvent.Raise(this, playerListeningForInput);
    }
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

