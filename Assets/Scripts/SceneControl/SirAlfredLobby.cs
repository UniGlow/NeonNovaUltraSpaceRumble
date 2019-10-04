using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class SirAlfredLobby : LevelManager 
{

    #region Variable Declaration
    [Header("Miscellaneous")]
    [SerializeField] float timeTillLevelStart = 1f;
    [SerializeField] float timeTillIdle = 5f;
    [SerializeField] PlayerReadyUpdater playerReadyUpdater;
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] MusicTrack backgroundTrack;
    [Range(0f,2f)]
    [SerializeField] float sfxVolumeDamp = 0.1f;
    [SerializeField] Points points = null;

    [Header("Player Configs")]
    [SerializeField] PlayerConfig hero1PlayerConfig = null;
    [SerializeField] PlayerConfig hero2PlayerConfig = null;
    [SerializeField] PlayerConfig hero3PlayerConfig = null;
    [SerializeField] PlayerConfig bossPlayerConfig = null;

    [Header("Color Set")]
    [SerializeField] ColorSet colorSet = null;

    [Header("Ability Set")]
    [SerializeField] Ability damageAbility = null;
    [SerializeField] Ability tankAbility = null;
    [SerializeField] Ability victimAbility = null;

    List<bool> playerConfirms = new List<bool>();
    float idleTimer;
    float originalSFXVolume;
    float originalMusicVolume;
    bool idleState;
    int playerCount;
    bool sceneLoadTriggered = false;
    #endregion



    #region Unity Event Functions
    private void Start () 
	{
        masterMixer.GetFloat(Constants.MIXER_SFX_VOLUME, out originalSFXVolume);
        masterMixer.GetFloat(Constants.MIXER_MUSIC_VOLUME, out originalMusicVolume);

        masterMixer.SetFloat(Constants.MIXER_SFX_VOLUME, originalSFXVolume * (1 + sfxVolumeDamp));
        StartCoroutine(Wait(0.1f, () => { AudioManager.Instance.StartTrack(backgroundTrack); }));
    }
	
	private void Update ()
    {
        // Update player count
        UpdatePlayerCount();
        UpdatePlayerConfirmsList();
        playerReadyUpdater.UpdateUIElements(playerCount);

        // Load next scene if all players are ready
        int ready = 0;
        for (int i = 0; i < playerConfirms.Count; i++)
        {
            if (playerConfirms[i] == true) ready++;
        }
        if (!sceneLoadTriggered && ready == playerConfirms.Count && playerConfirms.Count != 0)
        {
            sceneLoadTriggered = true;
            AudioManager.Instance.StopPlaying();
            StartCoroutine(Wait(1f, () => {
                // Rstore SFX audio level
                masterMixer.SetFloat(Constants.MIXER_SFX_VOLUME, originalSFXVolume);

                SceneManager.Instance.LoadNextScene();
            }));
        }

        // Check Inputs to ready up
		if (Input.GetButtonDown(Constants.INPUT_SUBMIT + "1"))
        {
            playerConfirms[0] = !playerConfirms[0];
            playerReadyUpdater.UpdateState(0, playerConfirms[0]);
        }
        else if (Input.GetButtonDown(Constants.INPUT_SUBMIT + "2"))
        {
            playerConfirms[1] = !playerConfirms[1];
            playerReadyUpdater.UpdateState(1, playerConfirms[1]);
        }
        else if (Input.GetButtonDown(Constants.INPUT_SUBMIT + "3"))
        {
            playerConfirms[2] = !playerConfirms[2];
            playerReadyUpdater.UpdateState(2, playerConfirms[2]);
        }
        else if (Input.GetButtonDown(Constants.INPUT_SUBMIT + "4"))
        {
            playerConfirms[3] = !playerConfirms[3];
            playerReadyUpdater.UpdateState(3, playerConfirms[3]);
        }

        // Check and update Idle State of the game
        UpdateIdleState();
    }
    #endregion



    #region Public Functions
    public void Initialize()
    {
        // Update player count
        UpdatePlayerCount();

        // Set playerNumbers depending on amount of human players
        switch (playerCount)
        {
            case 1:
                bossPlayerConfig.Initialize(1, Faction.Boss, colorSet.GetRandomColor(), false);
                hero1PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color1, true);
                hero1PlayerConfig.ability = damageAbility;
                hero2PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color2, true);
                hero2PlayerConfig.ability = tankAbility;
                hero3PlayerConfig.Initialize(4, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.ability = victimAbility;
                break;

            case 2:
                hero1PlayerConfig.Initialize(1, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                hero2PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                hero3PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.ability = victimAbility;
                bossPlayerConfig.Initialize(4, Faction.Boss, colorSet.GetRandomColor(), true);
                break;

            case 3:
                hero1PlayerConfig.Initialize(1, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                hero2PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                hero3PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.ability = victimAbility;
                bossPlayerConfig.Initialize(4, Faction.Boss, colorSet.GetRandomColor(), true);
                break;

            case 4:
                bossPlayerConfig.Initialize(1, Faction.Boss, colorSet.GetRandomColor(), false);
                hero1PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                hero2PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                hero3PlayerConfig.Initialize(4, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.ability = victimAbility;
                break;

            default:
                break;
        }

        GameManager.Instance.activeColorSet = colorSet;
        points.ResetPoints(true);

        UpdatePlayerConfirmsList();
    }

    public void UpdatePlayerCount()
    {
        playerCount = 0;
        string[] joystickNames = Input.GetJoystickNames();
        foreach (string name in joystickNames)
        {
            if (name != "")
            {
                playerCount++;
            }
        }
    }
    #endregion



    #region Inherited Functions
    protected override void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Initialize();

        RaiseLevelLoaded(timeTillLevelStart);

        Invoke("RaiseLevelStarted", timeTillLevelStart);
    }

    protected override void RaiseLevelLoaded(float levelStartDelay)
    {
        base.RaiseLevelLoaded(levelStartDelay);
    }

    protected override void RaiseLevelStarted()
    {
        base.RaiseLevelStarted();
    }
    #endregion



    #region Private Functions
    void UpdateIdleState()
    {
        if (Input.anyKey && idleState)
        {
            if (DOTween.IsTweening(this)) DOTween.Kill(this);

            // Fade in audio
            FadeAudio(Constants.MIXER_SFX_VOLUME, originalSFXVolume * (1 + sfxVolumeDamp), 1f);
            FadeAudio(Constants.MIXER_MUSIC_VOLUME, originalMusicVolume, 1f);

            idleTimer = 0f;
            idleState = false;
        }

        if (idleTimer >= timeTillIdle && !idleState)
        {
            // Fade out audio
            FadeAudio(Constants.MIXER_SFX_VOLUME, -80f, 3f);
            FadeAudio(Constants.MIXER_MUSIC_VOLUME, -7f, 3f);

            idleState = true;
        }

        if (Input.anyKey) idleTimer = 0f;
        else idleTimer += Time.deltaTime;
    }

    void FadeAudio(string mixerGroup, float to, float duration)
    {
        masterMixer.DOSetFloat(mixerGroup, to, duration).SetEase(Ease.InOutQuad).SetId(this);
    }

    void UpdatePlayerConfirmsList()
    {
        if (playerConfirms.Count == playerCount)
        {
            return;
        }

        playerConfirms.Clear();
        for (int i = 0; i < playerCount; i++)
        {
            playerConfirms.Add(false);
        }
    }
    #endregion



    #region Coroutines
    IEnumerator Wait(float duration, System.Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        onComplete.Invoke();
    }
    #endregion
}
