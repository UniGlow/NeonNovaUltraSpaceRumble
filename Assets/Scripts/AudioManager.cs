using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    AudioSource src;

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
            Debug.Log("There can only be one GameManager instantiated. Destroying this Instance...");
            Destroy(this);
        }
    }

    private void Start() {
        src = GetComponent<AudioSource>();

        StartBackgroundTrack();
	}
	
	private void Update() {
		
	}
    #endregion



    protected override void OnLevelCompleted() {
        src.Stop();
        src.PlayOneShot(levelEnd, levelEndVolume);
    }



    public void StartBackgroundTrack() {
        src.PlayOneShot(GetbackgroundTrack("Track01Intro"), GetbackgroundTrackVolume("Track01Intro"));
        StartCoroutine(StartAudioLoop());
    }



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
        yield return null;
        for (float i = 0; i < 3f; i+=Time.deltaTime) {
            yield return null;
            if (!src.isPlaying) {
                src.clip = GetbackgroundTrack("Track01Loop");
                src.loop = true;
                src.volume = GetbackgroundTrackVolume("Track01Loop");
                src.Play();
                break;
            }
        }
    }
}
