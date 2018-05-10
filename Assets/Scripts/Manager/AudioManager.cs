using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] AudioClip[] backgroundTracks;
    [Range(0,1)]
    [SerializeField] float[] backgroundTracksVolumes;
    [SerializeField] AudioClip levelEnd;
    [Range(0,1)]
    [SerializeField] float levelEndVolume = 1f;
    [SerializeField] AudioClip bossWinSound;
    [Range(0, 1)]
    [SerializeField] float bossWinSoundVolume = 1f;
    [SerializeField] AudioClip heroesWinSound;
    [Range(0, 1)]
    [SerializeField] float heroesWinSoundVolume = 1f;

    AudioSource audioSource;

    public static AudioManager Instance;
    #endregion



    #region Unity Event Functions
    void Awake() {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this) {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of an AudioManager.
            Debug.Log("There can only be one AudioManager instantiated. Destroying this Instance...");
            Destroy(this);
        }
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
	}
	
	private void Update() {
		
	}
    #endregion



    #region Custom Event Functions
    override protected void OnLevelCompleted(string winner)
    {
        PlayWinSequence(winner);
    }

    protected override void OnLevelStarted()
    {
        
    }
    #endregion



    #region Public Functions
    public void PlayWinSequence(string winner)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(levelEnd, levelEndVolume);

        StartCoroutine(PlayWinSoundDelayed(winner));
    }

    public void StartTutorialTrack()
    {
        audioSource.clip = GetbackgroundTrack("ElectronicIntro");
        audioSource.volume = GetbackgroundTrackVolume("ElectronicIntro");
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StartBackgroundTrack() {
        audioSource.clip = GetbackgroundTrack("Track01Intro");
        audioSource.volume = GetbackgroundTrackVolume("Track01Intro");
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(StartAudioLoop());
    }

    public void StartBackgroundTrackPitched()
    {
        audioSource.clip = GetbackgroundTrack("Track01IntroPitched");
        audioSource.volume = GetbackgroundTrackVolume("Track01IntroPitched");
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(StartAudioLoop());
    }

    public void StopPlaying() {
        audioSource.Stop();
    }
    #endregion



    #region Private Functions
    AudioClip GetbackgroundTrack(string name) {
        foreach (AudioClip clip in backgroundTracks) {
            if (clip.name == name) {
                return clip;
            }
        }
        return null;
    }

    float GetbackgroundTrackVolume(string name) {
        for (int i = 0; i < backgroundTracks.Length; i++) {
            if (backgroundTracks[i].name == name) {
                return backgroundTracksVolumes[i];
            }
        }
        return 1f;
    }
    #endregion



    IEnumerator StartAudioLoop() {
        for (float i = 0; i < audioSource.clip.length + 0.5f; i+=Time.deltaTime) {
            yield return null;
            if (!audioSource.isPlaying) {
                audioSource.clip = GetbackgroundTrack("Track01Loop");
                audioSource.loop = true;
                audioSource.volume = GetbackgroundTrackVolume("Track01Loop");
                audioSource.Play();
                break;
            }
        }
    }

    IEnumerator PlayWinSoundDelayed(string winner) {
        yield return new WaitForSecondsRealtime(levelEnd.length);

        audioSource.loop = false;
        if (winner == "Heroes")
        {
            audioSource.clip = heroesWinSound;
            audioSource.volume = heroesWinSoundVolume;
        }
        else if (winner == "Boss")
        {
            audioSource.clip = bossWinSound;
            audioSource.volume = bossWinSoundVolume;
        }

        audioSource.Play();
    }
}
