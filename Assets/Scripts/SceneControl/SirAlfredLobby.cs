using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;
using Rewired;

/// <summary>
/// 
/// </summary>
public class SirAlfredLobby : MonoBehaviour 
{

    #region Variable Declaration
    [Header("Game Settings")]
    [SerializeField] GameSettings settings;

    [Header("Miscellaneous")]
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

                GameManager.Instance.LoadNextScene();
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
        UpdatePlayerCount();

        // Set playerNumbers depending on amount of human players
        switch (playerCount)
        {
            case 1:
                bossPlayerConfig.Initialize(ReInput.players.GetPlayer(0), 0, Faction.Boss, colorSet.GetRandomColor(), false);
                ReInput.players.GetPlayer(0).isPlaying = true;
                hero1PlayerConfig.Initialize(ReInput.players.GetPlayer(1), 1, Faction.Heroes, colorSet.color1, true);
                hero1PlayerConfig.ability = damageAbility;
                ReInput.players.GetPlayer(1).isPlaying = false;
                hero2PlayerConfig.Initialize(ReInput.players.GetPlayer(2), 2, Faction.Heroes, colorSet.color2, true);
                hero2PlayerConfig.ability = tankAbility;
                ReInput.players.GetPlayer(2).isPlaying = false;
                hero3PlayerConfig.Initialize(ReInput.players.GetPlayer(3), 3, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.ability = victimAbility;
                ReInput.players.GetPlayer(3).isPlaying = false;
                break;

            case 2:
                hero1PlayerConfig.Initialize(ReInput.players.GetPlayer(0), 0, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                ReInput.players.GetPlayer(0).isPlaying = true;
                hero2PlayerConfig.Initialize(ReInput.players.GetPlayer(1), 1, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                ReInput.players.GetPlayer(1).isPlaying = true;
                hero3PlayerConfig.Initialize(ReInput.players.GetPlayer(2), 2, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.ability = victimAbility;
                ReInput.players.GetPlayer(2).isPlaying = false;
                bossPlayerConfig.Initialize(ReInput.players.GetPlayer(3), 3, Faction.Boss, colorSet.GetRandomColor(), true);
                ReInput.players.GetPlayer(3).isPlaying = false;
                break;

            case 3:
                hero1PlayerConfig.Initialize(ReInput.players.GetPlayer(0), 0, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                ReInput.players.GetPlayer(0).isPlaying = true;
                hero2PlayerConfig.Initialize(ReInput.players.GetPlayer(1), 1, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                ReInput.players.GetPlayer(1).isPlaying = true;
                hero3PlayerConfig.Initialize(ReInput.players.GetPlayer(2), 2, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.ability = victimAbility;
                ReInput.players.GetPlayer(2).isPlaying = true;
                bossPlayerConfig.Initialize(ReInput.players.GetPlayer(3), 3, Faction.Boss, colorSet.GetRandomColor(), true);
                ReInput.players.GetPlayer(3).isPlaying = false;
                break;

            case 4:
                bossPlayerConfig.Initialize(ReInput.players.GetPlayer(0), 0, Faction.Boss, colorSet.GetRandomColor(), false);
                ReInput.players.GetPlayer(0).isPlaying = true;
                hero1PlayerConfig.Initialize(ReInput.players.GetPlayer(1), 1, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                ReInput.players.GetPlayer(1).isPlaying = true;
                hero2PlayerConfig.Initialize(ReInput.players.GetPlayer(2), 2, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                ReInput.players.GetPlayer(2).isPlaying = true;
                hero3PlayerConfig.Initialize(ReInput.players.GetPlayer(3), 3, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.ability = victimAbility;
                ReInput.players.GetPlayer(3).isPlaying = true;
                break;

            default:
                break;
        }

        GameManager.Instance.activeColorSet = colorSet;
        points.ResetPoints(true);

        UpdatePlayerConfirmsList();
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

    void UpdatePlayerCount()
    {
        playerCount = 0;

        foreach (Player player in ReInput.players.Players)
        {
            if (player.isPlaying) playerCount++;
        }
    }

    void UpdatePlayerConfirmsList()
    {
        UpdatePlayerCount();

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
