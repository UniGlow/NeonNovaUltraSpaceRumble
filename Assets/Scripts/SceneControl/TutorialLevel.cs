﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class TutorialLevel : MonoBehaviour 
{

    #region Variable Declaration
    [Space]
    [SerializeField] float timeTillIdle = 5f;
    [SerializeField] PlayerReadyUpdater playerReadyUpdater;
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] MusicTrack backgroundTrack;
    [Range(0f,2f)]
    [SerializeField] float sfxVolumeDamp = 0.1f;

    List<bool> playerConfirms = new List<bool>();
    float idleTimer;
    float originalSFXVolume;
    float originalMusicVolume;
    bool idleState;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        UpdatePlayerConfirmsList();
        masterMixer.GetFloat(Constants.MIXER_SFX_VOLUME, out originalSFXVolume);
        masterMixer.GetFloat(Constants.MIXER_MUSIC_VOLUME, out originalMusicVolume);

        masterMixer.SetFloat(Constants.MIXER_SFX_VOLUME, originalSFXVolume * (1 + sfxVolumeDamp));
        StartCoroutine(Wait(0.1f, () => { AudioManager.Instance.StartTrack(backgroundTrack); }));
    }
	
	private void Update () 
	{
        // Update player count
        GameManager.Instance.UpdatePlayerCount();
        UpdatePlayerConfirmsList();
        playerReadyUpdater.UpdateUIElements(GameManager.Instance.PlayerCount);

        // Load next scene if all players are ready
        int ready = 0;
        for (int i = 0; i < playerConfirms.Count; i++)
        {
            if (playerConfirms[i] == true) ready++;
        }
        if (ready == playerConfirms.Count)
        {
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

        // Reset Texts on (Back) Button
        if (Input.GetButtonDown(Constants.INPUT_RESET)) ResetTexts();

        // Check and update Idle State of the game
        UpdateIdleState();
    }
    #endregion



    #region Private Functions
    void UpdateIdleState()
    {
        if (Input.anyKey && idleState)
        {
            if (LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);

            // Fade in audio
            float currentSFXVolume;
            masterMixer.GetFloat(Constants.MIXER_SFX_VOLUME, out currentSFXVolume);
            FadeAudio(Constants.MIXER_SFX_VOLUME, currentSFXVolume, originalSFXVolume * (1 + sfxVolumeDamp), 1f);
            float currentMusicVolume;
            masterMixer.GetFloat(Constants.MIXER_MUSIC_VOLUME, out currentMusicVolume);
            FadeAudio(Constants.MIXER_MUSIC_VOLUME, currentMusicVolume, originalMusicVolume, 1f);

            // Reset all TutorialTexts
            ResetTexts();

            idleTimer = 0f;
            idleState = false;
        }

        if (idleTimer >= timeTillIdle && !idleState)
        {
            // Fade out audio
            FadeAudio(Constants.MIXER_SFX_VOLUME, originalSFXVolume * (1 + sfxVolumeDamp), -80f, 3f);
            FadeAudio(Constants.MIXER_MUSIC_VOLUME, originalMusicVolume, -7f, 3f);

            idleState = true;
        }

        if (Input.anyKey) idleTimer = 0f;
        else idleTimer += Time.deltaTime;
    }

    void FadeAudio(string mixerGroup, float from, float to, float duration)
    {
        LeanTween.value(gameObject, from, to, duration).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float value) => {
            masterMixer.SetFloat(mixerGroup, value);
        });
    }

    void ResetTexts()
    {
        TutorialTextUpdater.BossColorChange(0);
        GameManager.Instance.ResetPassedTimeForColorChange();
    }

    void UpdatePlayerConfirmsList()
    {
        if (playerConfirms.Count == GameManager.Instance.PlayerCount)
        {
            return;
        }

        playerConfirms.Clear();
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
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
