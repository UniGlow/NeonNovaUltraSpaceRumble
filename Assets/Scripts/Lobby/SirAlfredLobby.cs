using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// </summary>
public class SirAlfredLobby : MonoBehaviour
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
    public static SirAlfredLobby Instance = null;
    // Serialized Fields
    [Header("Game Events")]
    [SerializeField] GameEvent slotListeningForInputEvent = null;
    [SerializeField] GameEvent playerCharacterChangedLobbyEvent = null;
    [SerializeField] GameEvent readyToStartEvent = null;
    [SerializeField] GameEvent playerSelectedCharacterEvent = null;
    [SerializeField] GameEvent messagePlayer5Event = null;
    [SerializeField] GameEvent bestOfModeChangedEvent = null;

    [Header("References")]
    [Tooltip("The Scriptable Object that holds all available Colors in the Game")]
    [SerializeField] AvailableColors availableColors = null;
    [Tooltip("The Scriptable Object that will be used during the battles which only holds 3 Colors")]
    [SerializeField] ColorSet activeColorSet = null;
    [SerializeField] Points points = null;
    [SerializeField] GameSettings gameSettings = null;
    [Space]
    [SerializeField] SceneReference lobbyUI = null;

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

    [Header("General Settings")]
    [Tooltip("Determines how long a Player has to hold B to return to the Main Menu")]
    [SerializeField] float abortPressDuration = 1.5f;
    [Tooltip("Time between possible Selections while choosing Best-of-Variation")]
    [SerializeField] float changeTimer = 0.5f;

    [Header("Sounds")]
    [SerializeField] AudioClip readyToFight = null;
    [Range(0, 1)]
    [SerializeField] float readyToFightVolume = 1f;

    // Private
    // Player Settings for PlayerConfigs
    PlayerSettings[] players = new PlayerSettings[4] { new PlayerSettings(), new PlayerSettings(), new PlayerSettings(), new PlayerSettings() };

    // Data
    List<PlayerColor> availablePlayerColors = new List<PlayerColor>();
    PlayerCharacter[] lastPlayerCharacters = new PlayerCharacter[4] { PlayerCharacter.Empty, PlayerCharacter.Empty, PlayerCharacter.Empty, PlayerCharacter.Empty };
    bool gameReadyToStart = false;
    float abortTimer = 0f;
    float selectionTimer = 0f;
    AudioSource audioSource = null;

    // Best-Of-Selection
    private int bestOfSelectedElement = 0;
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
#if UNITY_EDITOR
        if(UnityEngine.SceneManagement.SceneManager.sceneCount == 1)
        {
            SceneManager.Instance.LoadUIAdditive(lobbyUI);
        }
#endif

        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;
        InputHelper.ChangeRuleSetForAllPlayers(RewiredConsts.LayoutManagerRuleSet.RULESETLOBBY);
        RaiseSlotsListeningForInputs(MakeListeningForInputArray(true));
        foreach(PlayerColor pc in availableColors.PlayerColors)
        {
            availablePlayerColors.Add(pc);
        }
        UpdateAvailableCharacters(PlayerCharacter.Empty);

        // Reset Points
        points.ResetPoints();
        // Delete last Score Session
        if(bossConfig.HeroScore != null)
            bossConfig.HeroScore.ClearAllLevels();
        hero1Config.HeroScore.ClearAllLevels();
        hero2Config.HeroScore.ClearAllLevels();
        hero3Config.HeroScore.ClearAllLevels();

        AudioManager.Instance.StartTutorialTrack();

        if(gameSettings.UseBestOfFeature)
            bestOfSelectedElement = gameSettings.BestOfRange.Count / 2;
    }

    private void Update()
    {
        if (InputHelper.GetButton(RewiredConsts.Action.UICANCEL))
        {
            abortTimer += Time.deltaTime;
        }
        else
        {
            abortTimer = 0f;
        }
        if (abortTimer > abortPressDuration)
        {
            SceneManager.Instance.LoadMainMenu();
        }
        if (gameReadyToStart)
        {
            if(gameSettings.UseBestOfFeature)
                if(selectionTimer <= changeTimer)
                    selectionTimer += Time.deltaTime;
            //bool humans = false;
            foreach (PlayerSettings player in players)
            {
                if (player.player != null)
                {
                    //humans = true;
                    if (player.player.GetButtonDown(RewiredConsts.Action.UISTART))
                    {
                        SetupPlayerConfigs();
                        AudioManager.Instance.StopPlaying();
                        SceneManager.Instance.StartNextLevel();
                    }

                    // Select Best-of-Mode
                    bool changed = false;

                    if (selectionTimer < changeTimer)
                        return;
                    if (gameSettings.UseBestOfFeature)
                    {
                        if (player.player.GetAxis(RewiredConsts.Action.UIHORIZONTAL) > 0)
                        {
                            if (bestOfSelectedElement != (gameSettings.BestOfRange.Count - 1))
                            {
                                bestOfSelectedElement += 1;
                                changed = true;
                                selectionTimer = 0f;
                            }
                        }

                        else if (player.player.GetAxis(RewiredConsts.Action.UIHORIZONTAL) < 0)
                        {
                            if (bestOfSelectedElement != 0)
                            {
                                bestOfSelectedElement -= 1;
                                changed = true;
                                selectionTimer = 0f;
                            }
                        }

                        if (changed)
                        {
                            RaiseBestOfModeChanged(gameSettings.BestOfRange[bestOfSelectedElement].ToString());
                        }
                    }
                }
            }
            /* TODO: After AI-Rework, implement this to be able to Start a Game with nothing but AI
            if (!humans)
            {
                if (InputHelper.GetButtonDown(RewiredConsts.Action.UISTART))
                {
                    SetupPlayerConfigs();
                    SceneManager.Instance.LoadNextLevel();
                }
            }*/
        }

        // Message to Player 5
        Rewired.Player p = InputHelper.GetPlayerButtonDown(RewiredConsts.Action.UISUBMIT);
        if (p != null)
        {
            bool player5 = true;
            for (int i = 0; i < 4; i++)
            {
                if (p == players[i].player)
                    player5 = false;
            }
            if (player5)
                RaiseMessagePlayer5();
        }
    }
    #endregion



    #region Public Functions
    /// <summary>
    /// Call this if a Player has entered a new State in the Lobby.
    /// </summary>
    /// <param name="panelNumber">Number of that Players Panel.</param>
    /// <param name="activeStep">The State that has been entered.</param>
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
            RaiseSlotsListeningForInputs(MakeListeningForInputArray());
            CheckReadyStates();
        }
    }

    /// <summary>
    /// Marks the given Rewired.Player as active and assigns it to the given Panel
    /// </summary>
    /// <param name="panelNumber">Number of the Panel in question</param>
    /// <param name="player">Rewired.Player that should be marked as active. If null is given, the previously active Player is set to inactive</param>
    public void SetPlayer(int panelNumber, Rewired.Player player)
    {
        players[panelNumber - 1].player = player;
        if (player != null)
            players[panelNumber - 1].playerNumber = panelNumber - 1;
        else
            players[panelNumber - 1].playerNumber = -1;
    }

    /// <summary>
    /// Returns true if the given Rewired.Player is marked as active in the Lobby.
    /// Returns false if not.
    /// </summary>
    /// <param name="player">The Rewired.Player in question</param>
    /// <returns></returns>
    public bool IsPlayerActive(Rewired.Player player)
    {
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].player == player)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Saves the active Color of that Player as selected and blocks that Color for all other Players.
    /// </summary>
    /// <param name="panelNumber">Panel Number of the Player</param>
    /// <param name="playerColor">Color that has been selected, if null is given
    /// the previously selected Color will be made available for all Players again</param>
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

    /// <summary>
    /// Sets a Players Ready-To-Play-Status to the given value.
    /// </summary>
    /// <param name="panelNumber">Players Panel Number</param>
    /// <param name="readyState">value</param>
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
    /// <param name="panelNumber">The panel number. If 0 a full List will be created.</param>
    public void UpdateAvailableCharacters (PlayerCharacter selectedCharacter, int panelNumber = 0)
    {
        if (panelNumber != 0)
        {
            players[panelNumber - 1].characterSelectionLoggedIn = selectedCharacter == PlayerCharacter.Empty ? false : true;
        }
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

    /// <summary>
    /// Returns the Starting Color of the Given Character
    /// </summary>
    /// <param name="character">Character of which the Color is needed</param>
    /// <returns></returns>
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
    /// <summary>
    /// Checks if all Players are Ready to Play and reacts to that Checks result appropriatly
    /// </summary>
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
        // TODO: After AI-Rework, delete playerCount != 0 Statement to be able to Start a Match with nothing but AI playing
        if (playerCount != 0 && playerCount == playersReady)
        {
            if (!gameReadyToStart)
            {
                gameReadyToStart = true;
                audioSource.PlayOneShot(readyToFight, readyToFightVolume);
                RaiseReadyToStart(true);
                RaiseBestOfModeChanged(gameSettings.BestOfRange[bestOfSelectedElement].ToString());
            }
        }
        else
        {
            gameReadyToStart = false;
            // TODO: Anzeige UI abschalten
            RaiseReadyToStart(false);
        }
    }

    /// <summary>
    /// Constructs an Array of Bools representing the Lobby Panels that are activly Listening for Inputs of all Controllers.
    /// </summary>
    /// <param name="start">When true the first Lobby-Panel will be set to Listening</param>
    /// <returns></returns>
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

    /// <summary>
    /// Sets up the PlayerConfigs with all the Information needed to Start the Game.
    /// All Human Players will be Set first, then AIs will be Setup to fill that Number up to 4.
    /// </summary>
    void SetupPlayerConfigs()
    {
        if(gameSettings.UseBestOfFeature)
            gameSettings.BestOf = gameSettings.BestOfRange[bestOfSelectedElement];
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
                        players[i].playerColor = FallbackColorCheck(i);
                        hero1Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero1Config.SetupAbility(damageAbility);
                        characters[i] = PlayerCharacter.Damage;
                        colorsNotToUse.Add(players[i].playerColor);
                        availablePlayers.Remove(players[i].player);
                        break;
                    case PlayerCharacter.Runner:
                        players[i].playerColor = FallbackColorCheck(i);
                        hero2Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero2Config.SetupAbility(runnerAbility);
                        characters[i] = PlayerCharacter.Runner;
                        colorsNotToUse.Add(players[i].playerColor);
                        availablePlayers.Remove(players[i].player);
                        break;
                    case PlayerCharacter.Tank:
                        players[i].playerColor = FallbackColorCheck(i);
                        hero3Config.Initialize(players[i].player, players[i].playerNumber, Faction.Heroes, players[i].playerColor, false);
                        hero3Config.SetupAbility(tankAbility);
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
                    hero1Config.SetupAbility(damageAbility);
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
                    hero2Config.SetupAbility(runnerAbility);
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
                    hero3Config.SetupAbility(tankAbility);
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

    /// <summary>
    /// Fallback Check to provent a Bug that could enable two Players to choose the same Color.
    /// </summary>
    /// <param name="position">Position in Players-Array up to which should be checked.</param>
    /// <returns>A valid free PlayerColor</returns>
    private PlayerColor FallbackColorCheck(int position)
    {
        for (int j = 0; j < position; j++)
        {
            if (players[position].playerColor == players[j].playerColor)
            {
                PlayerColor invalidColor = players[position].playerColor;
                Debug.LogError("A Player Color was taken twice. This shouldn't have happend! Fallback to find a free Color for Player: " + players[position].PlayerCharacter.ToString());
                for (int x = 0; x < availableColors.PlayerColors.Count; x++)
                {
                    bool colorIsFree = true;
                    for (int y = 0; y < 3; y++)
                    {
                        if (players[y].playerColor == availableColors.PlayerColors[x])
                            colorIsFree = false;
                    }
                    if (colorIsFree)
                        return availableColors.PlayerColors[x];
                }
                Debug.LogError("ERROR! Fallback Failed! There is no Color left to asign to this Player!");
            }
        }
        return players[position].playerColor;
    }

    private void ManageBestOfSelection()
    {

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

    void RaiseMessagePlayer5()
    {
        messagePlayer5Event.Raise(this);
    }

    void RaiseBestOfModeChanged(string text)
    {
        bestOfModeChangedEvent.Raise(this, text);
    }
    #endregion



    #region Coroutines

    #endregion
}
