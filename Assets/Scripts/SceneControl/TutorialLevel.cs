using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

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
    bool[] playerConfirms;
    float idleTimer;
    float originalSFXVolume;
    float originalMusicVolume;
    bool idleState;
    HomingMissile homingMissile;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        playerConfirms = new bool[GameManager.Instance.PlayerCount];
        masterMixer.GetFloat(Constants.MIXER_SFX_VOLUME, out originalSFXVolume);
        masterMixer.GetFloat(Constants.MIXER_MUSIC_VOLUME, out originalMusicVolume);
        homingMissile = GameObject.FindObjectOfType<HomingMissile>();
	}
	
	private void Update () 
	{
        // Load next scene if all players are ready
        int ready = 0;
        for (int i = 0; i < playerConfirms.Length; i++)
        {
            if (playerConfirms[i] == true) ready++;
        }
        if (ready == playerConfirms.Length)
        {
            AudioManager.Instance.StopPlaying();
            StartCoroutine(Wait(1f, () => { GameManager.Instance.LoadNextScene(); }));
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



    #region Private Functions
    void UpdateIdleState()
    {
        if (Input.anyKey)
        {
            if (LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);

            float currentSFXVolume;
            masterMixer.GetFloat(Constants.MIXER_SFX_VOLUME, out currentSFXVolume);
            LeanTween.value(gameObject, currentSFXVolume, originalSFXVolume, 1f).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float value) => {
                masterMixer.SetFloat(Constants.MIXER_SFX_VOLUME, value);
            });
            float currentMusicVolume;
            masterMixer.GetFloat(Constants.MIXER_MUSIC_VOLUME, out currentMusicVolume);
            LeanTween.value(gameObject, currentMusicVolume, originalMusicVolume, 1f).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float value) => {
                masterMixer.SetFloat(Constants.MIXER_MUSIC_VOLUME, value);
            });

            homingMissile.enableCameraShake = true;

            idleTimer = 0f;
            idleState = false;
        }

        if (idleTimer >= timeTillIdle && !idleState)
        {
            LeanTween.value(gameObject, originalSFXVolume, -80f, 3f).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float value) => {
                masterMixer.SetFloat(Constants.MIXER_SFX_VOLUME, value);
            });
            LeanTween.value(gameObject, originalMusicVolume, -7f, 3f).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float value) => {
                masterMixer.SetFloat(Constants.MIXER_MUSIC_VOLUME, value);
            });
            homingMissile.enableCameraShake = false;
            idleState = true;
        }

        idleTimer += Time.deltaTime;
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
