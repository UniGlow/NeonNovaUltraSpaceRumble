using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Rewired;

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

    protected override void InheritedOnEnable()
    {
        ReInput.ControllerConnectedEvent += ReloadLobby;
        ReInput.ControllerDisconnectedEvent += ReloadLobby;
    }

    protected override void InheritedOnDisable()
    {
        ReInput.ControllerConnectedEvent -= ReloadLobby;
        ReInput.ControllerDisconnectedEvent -= ReloadLobby;
    }

    private void Update ()
    {
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

        // Check inputs for Ready-Ups
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            if (ReInput.players.Players[i].GetButtonDown(RewiredConsts.Action.READY_UP))
            {
                playerConfirms[i] = !playerConfirms[i];
                playerReadyUpdater.UpdateState(i, playerConfirms[i]);
            }
        }

        // Check and update Idle State of the game
        UpdateIdleState();
    }
    #endregion



    #region Public Functions
    public void Initialize()
    {
        playerCount = InputHelper.UpdatePlayerCount();

        // Set playerNumbers depending on amount of human players
        PlayerSetup.SetupPlayers(playerCount, bossPlayerConfig, hero1PlayerConfig, hero2PlayerConfig, hero3PlayerConfig, damageAbility, tankAbility, victimAbility, colorSet);

        points.ResetPoints(true);

        UpdatePlayerConfirmsList();
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
        playerCount = InputHelper.UpdatePlayerCount();

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

    void ReloadLobby(ControllerStatusChangedEventArgs args)
    {
        SceneManager.Instance.ReloadLevel();
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
