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
        public bool characterSelectionLoggedIn = false;
        public int playerNumber = -1; // THIS IS STARTING AT 0!!! NOT THE PANELNUMBER!

        // For Alfred
        public bool listeningForInput = false;
        public bool active = false;

        public void Clear()
        {
            player = null;
            readyToPlay = false;
            playerColor = null;
            PlayerCharacter = PlayerCharacter.Empty;
            playerNumber = -1;
            listeningForInput = false;
            active = false;
        }
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
    [Header("Game Events")]
    [SerializeField] GameEvent slotListeningForInputEvent = null;
    [SerializeField] GameEvent playerCharacterChangedLobbyEvent = null;
    [SerializeField] GameEvent readyToStartEvent = null;
    [SerializeField] GameEvent playerSelectedCharacterEvent = null;

    [Header("References")]
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

    [Header("Start Colors")]
    [SerializeField] PlayerColor damageHeroColor = null;
    [SerializeField] PlayerColor tankHeroColor = null;
    [SerializeField] PlayerColor runnerHeroColor = null;
    // Private
    // Player Settings for PlayerConfigs
    PlayerSettings[] players = new PlayerSettings[4] { new PlayerSettings(), new PlayerSettings(), new PlayerSettings(), new PlayerSettings() };
    
    // Data
    List<PlayerColor> availablePlayerColors = new List<PlayerColor>();
    PlayerCharacter[] lastPlayerCharacters = new PlayerCharacter[4] { PlayerCharacter.Empty, PlayerCharacter.Empty, PlayerCharacter.Empty, PlayerCharacter.Empty };
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
        foreach(PlayerColor pc in availableColors.PlayerColors)
        {
            availablePlayerColors.Add(pc);
        }
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
                    SetupPlayerConfigs();
                    SceneManager.Instance.LoadNextLevel();

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
            // Save last selected Character
            lastPlayerCharacters[panelNumber - 1] = players[panelNumber - 1].PlayerCharacter;
            // Clear all Data of that Player
            players[panelNumber - 1].Clear();
            // Make that Slot available for all Players again
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
            if(players[panelNumber-1].PlayerCharacter == PlayerCharacter.Empty)
            {
                if (lastPlayerCharacters[panelNumber-1] != PlayerCharacter.Empty)
                {
                    players[panelNumber - 1].PlayerCharacter = lastPlayerCharacters[panelNumber - 1];
                }
                else
                {
                    players[panelNumber - 1].PlayerCharacter = PlayerCharacter.Boss;
                }
            }
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
        if (player != null)
            players[panelNumber - 1].playerNumber = panelNumber - 1;
        else
            players[panelNumber - 1].playerNumber = -1;
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

    // Used directly by SelectionControllers NOT by GameEvent
    public void SetPlayerColor(int panelNumber, PlayerColor playerColor)
    {
        if (playerColor != null)
        {
            availablePlayerColors.Remove(playerColor);
        }
        else
        {
            availablePlayerColors.Add(players[panelNumber-1].playerColor);
        }
        players[panelNumber - 1].playerColor = playerColor;
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

    /// <summary>
    /// Sets the current Player Character to the next Character in the Wheel
    /// </summary>
    /// <param name="panelNumber"></param>
    /// <param name="direction">Direction of Turning the Wheel, Left or Right</param>
    /// <returns>True if Player is Boss, False if Player isn't</returns>
    public PlayerCharacter SwitchPlayerCharacter(int panelNumber, Direction direction)
    {
        // TODO: Hardcode-Reihenfolge entfernen und dynamische Lösung finden
        switch (players[panelNumber - 1].PlayerCharacter)
        {
            case PlayerCharacter.Boss:
                players[panelNumber - 1].PlayerCharacter = direction == Direction.Left ? PlayerCharacter.Tank : PlayerCharacter.Damage;
                break;
            case PlayerCharacter.Damage:
                players[panelNumber - 1].PlayerCharacter = direction == Direction.Left ? PlayerCharacter.Boss : PlayerCharacter.Runner;
                break;
            case PlayerCharacter.Runner:
                players[panelNumber - 1].PlayerCharacter = direction == Direction.Left ? PlayerCharacter.Damage : PlayerCharacter.Tank;
                break;
            case PlayerCharacter.Tank:
                players[panelNumber - 1].PlayerCharacter = direction == Direction.Left ? PlayerCharacter.Runner : PlayerCharacter.Boss;
                break;
        }
        RaisePlayerCharacterChangedLobby(panelNumber, players[panelNumber - 1].PlayerCharacter);

        return players[panelNumber - 1].PlayerCharacter;
    }

    /// <summary>
    /// Updates the available characters. Gets called by SelectionController.
    /// </summary>
    /// <param name="selectedCharacter">The selected character.</param>
    /// <param name="panelNumber">The panel number.</param>
    public void UpdateAvailableCharacters (PlayerCharacter selectedCharacter, int panelNumber)
    {
        players[panelNumber - 1].PlayerCharacter = selectedCharacter;
        players[panelNumber - 1].characterSelectionLoggedIn = selectedCharacter == PlayerCharacter.Empty ? false : true;

        // Create return list
        List<PlayerCharacter> availableCharacters = new List<PlayerCharacter>();
        availableCharacters.Add(PlayerCharacter.Boss);
        availableCharacters.Add(PlayerCharacter.Damage);
        availableCharacters.Add(PlayerCharacter.Tank);
        availableCharacters.Add(PlayerCharacter.Runner);

        // delete currently logged in characters
        foreach (PlayerSettings player in players)
        {
            if (player.characterSelectionLoggedIn) availableCharacters.Remove(player.PlayerCharacter);
        }

        RaisePlayerSelectedCharacter(availableCharacters);
    }

    public PlayerColor GetStartingColor(PlayerCharacter character)
    {
        switch (character)
        {
            case PlayerCharacter.Damage:
                return damageHeroColor;
            case PlayerCharacter.Runner:
                return runnerHeroColor;
            case PlayerCharacter.Tank:
                return tankHeroColor;
            default:
                return null;
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
            RaiseReadyToStart(true);
        }
        else
        {
            gameReadyToStart = false;
            // TODO: Anzeige UI abschalten
            Debug.LogAssertion("Game NOT Ready to Start!");
            RaiseReadyToStart(false);
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
        // TODO: TEST THIS WITH 5 CONTROLLERS ON THE SAME MACHINE!!!!!!!!!!!!!!!
        List<Rewired.Player> availablePlayers = new List<Rewired.Player>();
        availablePlayers.Add(Rewired.ReInput.players.GetPlayer(0));
        availablePlayers.Add(Rewired.ReInput.players.GetPlayer(1));
        availablePlayers.Add(Rewired.ReInput.players.GetPlayer(2));
        availablePlayers.Add(Rewired.ReInput.players.GetPlayer(3));
        Rewired.Player bossplayer = null;
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].player != null)
            {
                switch (players[i].PlayerCharacter)
                {
                    case PlayerCharacter.Boss:
                        // Boss has to be Initialised last! ColorSet has to be filled first!
                        bossplayer = players[i].player;
                        availablePlayers.Remove(players[i].player);
                        characters[i] = PlayerCharacter.Boss;
                        break;
                    case PlayerCharacter.Damage:
                        hero1Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero1Config.ability = damageAbility;
                        characters[i] = PlayerCharacter.Damage;
                        colorsNotToUse.Add(players[i].playerColor);
                        availablePlayers.Remove(players[i].player);
                        break;
                    case PlayerCharacter.Runner:
                        hero2Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero2Config.ability = runnerAbility;
                        characters[i] = PlayerCharacter.Runner;
                        colorsNotToUse.Add(players[i].playerColor);
                        availablePlayers.Remove(players[i].player);
                        break;
                    case PlayerCharacter.Tank:
                        hero3Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero3Config.ability = tankAbility;
                        characters[i] = PlayerCharacter.Tank;
                        colorsNotToUse.Add(players[i].playerColor);
                        availablePlayers.Remove(players[i].player);
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
                    hero1Config.Initialize(availablePlayers[0], i, Faction.Heroes, color, true);
                    hero1Config.ability = damageAbility;
                    availablePlayers.RemoveAt(0);
                    characters[i] = PlayerCharacter.Damage;
                    colorsNotToUse.Add(color);
                }
                else if (!characters.Contains(PlayerCharacter.Runner))
                {
                    PlayerColor color = colorsNotToUse.Count == 0 ? availableColors.GetRandomColorExcept() :
                        (colorsNotToUse.Count == 1 ? availableColors.GetRandomColorExcept(colorsNotToUse[0]) :
                            (colorsNotToUse.Count == 2 ? availableColors.GetRandomColorExcept(colorsNotToUse[0], colorsNotToUse[1]) : availablePlayerColors[0]));
                    hero2Config.Initialize(availablePlayers[0], i, Faction.Heroes, color, true);
                    hero2Config.ability = runnerAbility;
                    availablePlayers.RemoveAt(0);
                    characters[i] = PlayerCharacter.Runner;
                    colorsNotToUse.Add(color);
                }
                else if (!characters.Contains(PlayerCharacter.Tank))
                {
                    PlayerColor color = colorsNotToUse.Count == 0 ? availableColors.GetRandomColorExcept() :
                        (colorsNotToUse.Count == 1 ? availableColors.GetRandomColorExcept(colorsNotToUse[0]) :
                            (colorsNotToUse.Count == 2 ? availableColors.GetRandomColorExcept(colorsNotToUse[0], colorsNotToUse[1]) : availablePlayerColors[0]));
                    hero3Config.Initialize(availablePlayers[0], i, Faction.Heroes, color, true);
                    hero3Config.ability = tankAbility;
                    availablePlayers.RemoveAt(0);
                    characters[i] = PlayerCharacter.Tank;
                    colorsNotToUse.Add(color);
                }
            }
        }
        // Setup Active ColorSet
        activeColorSet.color1 = hero1Config.ColorConfig;
        activeColorSet.color2 = hero2Config.ColorConfig;
        activeColorSet.color3 = hero3Config.ColorConfig;

        // Initialise Boss here
        for(int i=0; i<characters.Count; i++)
        {
            if(characters[i] == PlayerCharacter.Empty)
            {
                bossConfig.Initialize(availablePlayers[0], players[i].playerNumber, Faction.Boss, activeColorSet.GetRandomColor(), true);
            }
            if(characters[i] == PlayerCharacter.Boss)
            {
                bossConfig.Initialize(players[i].player, players[i].playerNumber, Faction.Boss, activeColorSet.GetRandomColor(), false);
            }
        }

        // Setup GameManager
        GameManager.Instance.activeColorSet = activeColorSet;
        GameManager.Instance.IsInitialized = true;
    }
	#endregion
	
	
	
	#region GameEvent Raiser
	void RaiseSlotsListeningForInputs(bool[] playerListeningForInput)
    {
        slotListeningForInputEvent.Raise(this, playerListeningForInput);
    }

    void RaisePlayerCharacterChangedLobby(int panelNumber, PlayerCharacter activeCharacter)
    {
        playerCharacterChangedLobbyEvent.Raise(this, panelNumber, activeCharacter);
    }

    void RaiseReadyToStart(bool value)
    {
        readyToStartEvent.Raise(this, value);
    }

    void RaisePlayerSelectedCharacter(List<PlayerCharacter> availableCharacters)
    {
        playerSelectedCharacterEvent.Raise(this, availableCharacters);
    }
    #endregion



    #region Coroutines

    #endregion
}