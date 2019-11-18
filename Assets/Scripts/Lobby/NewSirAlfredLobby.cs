using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class NewSirAlfredLobby : MonoBehaviour 
{
    [System.Serializable]
    class PlayerSettings
    {
        // For PlayerConfig
        public Rewired.Player player = null;
        public bool readyToPlay = false;
        public PlayerColor playerColor = null;
        public PlayerCharacter PlayerCharacter = PlayerCharacter.Empty;
        public int playerNumber; // THIS IS STARTING AT 0!!! NOT THE PANELNUMBER!

        // For Alfred
        public bool listeningForInput = false;
        public bool active = false;
    }

    public enum PlayerCharacter
    {
        Empty,
        Boss,
        Tank,
        Damage,
        Runner
    }

    #region Variable Declarations
    public static NewSirAlfredLobby Instance = null;
    // Serialized Fields
    [SerializeField] GameEvent slotListeningForInputEvent = null;
    [Tooltip("The Scriptable Object that holds all available Colors in the Game")]
    [SerializeField] AvailableColors availableColors = null;
    [Tooltip("The Scriptable Object that will be used during the battles which only holds 3 Colors")]
    [SerializeField] ColorSet activeColorSet = null;

    [Header("PlayerConfigs")]
    [SerializeField] PlayerConfig bossConfig = null;
    [SerializeField] PlayerConfig hero1Config = null;
    [SerializeField] PlayerConfig hero2Config = null;
    [SerializeField] PlayerConfig hero3Config = null;

    [Header("Abilities")]
    [SerializeField] Ability damageAbility = null;
    [SerializeField] Ability tankAbility = null;
    [SerializeField] Ability runnerAbility = null;
    // Private
    // Player Settings for PlayerConfigs
    PlayerSettings[] players = new PlayerSettings[4] { new PlayerSettings(), new PlayerSettings(), new PlayerSettings(), new PlayerSettings() };
    
        // Data
    List<PlayerColor> availablePlayerColors = new List<PlayerColor>();
    bool gameReadyToStart = false;
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
        RaiseSlotsListeningForInputs(MakeListeningForInputArray(true));
        availablePlayerColors = availableColors.playerColors;
    }

    private void Update()
    {
        if (gameReadyToStart)
        {
            foreach (PlayerSettings player in players)
            {
                if (player.player != null && player.player.GetButtonDown(RewiredConsts.Action.UISTART))
                {
                    // TODO: Setup PlayerConfigs and load first Level
                    Debug.LogAssertion("Game Started!");
                }
            }
        }
    }
    #endregion



    #region Public Functions
    public void PlayerChangedState(int panelNumber, SelectionController.Step activeStep)
    {
        bool somethingChanged = false;
        if(activeStep == SelectionController.Step.Offline)
        {
            players[panelNumber - 1].listeningForInput = true;
            players[panelNumber - 1].active = false;
            bool firstElementChanged = false;
            for(int i = 0; i < players.Length; i++)
            {
                if (!firstElementChanged)
                {
                    if (players[i].listeningForInput)
                    {
                        firstElementChanged = true;
                    }
                }
                else
                {
                    players[i].listeningForInput = false;
                }
            }
            somethingChanged = true;
        }
        else if(activeStep == SelectionController.Step.CharacterSelection)
        {
            players[panelNumber - 1].listeningForInput = false;
            players[panelNumber - 1].active = true;
            bool firstElementChanged = false;
            for (int i = 0; i < players.Length; i++)
            {
                if (!firstElementChanged)
                {
                    if (!players[i].listeningForInput && !players[i].active)
                    {
                        players[i].listeningForInput = true;
                        firstElementChanged = true;
                    }
                }
                else
                {
                    players[i].listeningForInput = false;
                }
            }
            somethingChanged = true;
        }
        if (somethingChanged)
        {
            string log = "Slots listening for Input:\n";
            for (int i = 0; i < players.Length; i++)
                log += "P" + (i+1) + ": " + players[i].listeningForInput + "\n";
            Debug.Log(log);
            RaiseSlotsListeningForInputs(MakeListeningForInputArray());
            CheckReadyStates();
        }
    }

    public void SetPlayer(int panelNumber, Rewired.Player player)
    {
        players[panelNumber - 1].player = player;
    }

    public bool IsPlayerActive(Rewired.Player player)
    {
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].player == player)
                return true;
        }
        return false;
    }

    public void SetPlayerColor(int panelNumber, PlayerColor playerColor)
    {
        players[panelNumber - 1].playerColor = playerColor;
        if (playerColor != null)
        {
            availablePlayerColors.Remove(playerColor);
        }
        else
        {
            availablePlayerColors.Add(playerColor);
        }
    }

    public void SetReadyToPlay(int panelNumber, bool readyState)
    {
        players[panelNumber - 1].readyToPlay = readyState;
        CheckReadyStates();
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
	void CheckReadyStates()
    {
        int playerCount = 0;
        int playersReady = 0;
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].active)
                playerCount++;
        }
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].readyToPlay)
                playersReady++;
        }
        if (playerCount == playersReady)
        {
            gameReadyToStart = true;
            // TODO: Anzeige UI
            Debug.LogAssertion("Game Ready to Start!");
        }
        else
        {
            gameReadyToStart = false;
            // TODO: Anzeige UI abschalten
            Debug.LogAssertion("Game NOT Ready to Start!");
        }
    }

    bool[] MakeListeningForInputArray(bool start = false)
    {
        bool[] playerListeningForInput = new bool[4] { false, false, false, false };
        if (start)
            players[0].listeningForInput = true;
        for (int i = 0; i < players.Length; i++)
        {
            playerListeningForInput[i] = players[i].listeningForInput;
        }
        return playerListeningForInput;
    }

    void SetupPlayerConfigs()
    {
        List<PlayerCharacter> characters = new List<PlayerCharacter>();
        characters.Add(PlayerCharacter.Empty);
        characters.Add(PlayerCharacter.Empty);
        characters.Add(PlayerCharacter.Empty);
        characters.Add(PlayerCharacter.Empty);
        List<PlayerColor> colorsNotToUse = new List<PlayerColor>();
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].player != null)
            {
                switch (players[i].PlayerCharacter)
                {
                    case PlayerCharacter.Boss:
                        // Boss has to be Initialised last! ColorSet has to be filled first!
                        characters[i] = PlayerCharacter.Boss;
                        break;
                    case PlayerCharacter.Damage:
                        hero1Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero1Config.ability = damageAbility;
                        characters[i] = PlayerCharacter.Damage;
                        colorsNotToUse.Add(players[i].playerColor);
                        break;
                    case PlayerCharacter.Runner:
                        hero2Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero2Config.ability = runnerAbility;
                        characters[i] = PlayerCharacter.Runner;
                        colorsNotToUse.Add(players[i].playerColor);
                        break;
                    case PlayerCharacter.Tank:
                        hero3Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero3Config.ability = tankAbility;
                        characters[i] = PlayerCharacter.Tank;
                        colorsNotToUse.Add(players[i].playerColor);
                        break;
                }
            }
        }
        for(int i = 0; i < characters.Count; i++)
        {
            if(characters[i] == PlayerCharacter.Empty)
            {
                if (!characters.Contains(PlayerCharacter.Damage))
                {
                    PlayerColor color = colorsNotToUse.Count == 0 ? availableColors.GetRandomColorExcept() :
                        (colorsNotToUse.Count == 1 ? availableColors.GetRandomColorExcept(colorsNotToUse[0]) :
                            (colorsNotToUse.Count == 2 ? availableColors.GetRandomColorExcept(colorsNotToUse[0], colorsNotToUse[1]) : availablePlayerColors[0]));
                    hero1Config.Initialize(null, i, Faction.Heroes, color, true);
                    characters[i] = PlayerCharacter.Damage;
                    colorsNotToUse.Add(color);
                }
                else if (!characters.Contains(PlayerCharacter.Runner))
                {
                    PlayerColor color = colorsNotToUse.Count == 0 ? availableColors.GetRandomColorExcept() :
                        (colorsNotToUse.Count == 1 ? availableColors.GetRandomColorExcept(colorsNotToUse[0]) :
                            (colorsNotToUse.Count == 2 ? availableColors.GetRandomColorExcept(colorsNotToUse[0], colorsNotToUse[1]) : availablePlayerColors[0]));
                    hero2Config.Initialize(null, i, Faction.Heroes, color, true);
                    characters[i] = PlayerCharacter.Runner;
                    colorsNotToUse.Add(color);
                }
                else if (!characters.Contains(PlayerCharacter.Tank))
                {
                    PlayerColor color = colorsNotToUse.Count == 0 ? availableColors.GetRandomColorExcept() :
                        (colorsNotToUse.Count == 1 ? availableColors.GetRandomColorExcept(colorsNotToUse[0]) :
                            (colorsNotToUse.Count == 2 ? availableColors.GetRandomColorExcept(colorsNotToUse[0], colorsNotToUse[1]) : availablePlayerColors[0]));
                    hero3Config.Initialize(null, i, Faction.Heroes, color, true);
                    characters[i] = PlayerCharacter.Damage;
                    colorsNotToUse.Add(color);
                }
            }
        }
        // Setup Active ColorSet

        // Initialise Boss here
        for(int i=0; i<characters.Count; i++)
        {
            if(characters[i] == PlayerCharacter.Empty)
            {
                bossConfig.Initialize(null, players[i].playerNumber, Faction.Boss, activeColorSet.GetRandomColor(), true);
            }
            if(characters[i] == PlayerCharacter.Boss)
            {
                bossConfig.Initialize(players[i].player, players[i].playerNumber, Faction.Boss, activeColorSet.GetRandomColor(), false);
            }
        }
            
    }
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