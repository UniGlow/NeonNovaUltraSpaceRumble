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
    [SerializeField] List<MusicTrack> musicTracks = new List<MusicTrack>();

    [Space]
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
    AudioSource audioSourceSFX;
    bool startingTrack;

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
            Destroy(gameObject);
        }
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSourceSFX = transform.GetChild(0).GetComponent<AudioSource>();
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

    public void StartTrack(string name)
    {
        if (startingTrack) return;
        startingTrack = true;

        MusicTrack track = GetTrack(name);
        audioSource.clip = track.intro;
        audioSource.volume = track.volume;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(StartAudioLoop(track));
    }

    public void StartTrack(MusicTrack track)
    {
        if (startingTrack) return;
        startingTrack = true;

        audioSource.clip = track.intro;
        audioSource.volume = track.volume;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(StartAudioLoop(track));
    }

    public void StartTutorialTrack()
    {
        if (startingTrack) return;
        startingTrack = true;

        MusicTrack track = GetTrack("TutorialTrack");
        audioSource.clip = track.intro;
        audioSource.volume = track.volume;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(StartAudioLoop(track));
    }

    public void StartRandomTrack()
    {
        if (startingTrack) return;
        startingTrack = true;

        // Ignore OriginalTrack and TutorialTrack
        MusicTrack track = musicTracks[Random.Range(2, musicTracks.Count)];
        audioSource.clip = track.intro;
        audioSource.volume = track.volume;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(StartAudioLoop(track));

        GameObject.FindObjectOfType<SongTextUpdater>().ShowSongTitle(track.artist, track.title);
    }

    public void StopPlaying() {
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = 1f;
        audioSource.loop = false;
        StopAllCoroutines();
        startingTrack = false;
    }

    public void PlayClip(AudioClip clip, float volume)
    {
        audioSourceSFX.PlayOneShot(clip, volume);
    }
    #endregion



    #region Private Functions
    MusicTrack GetTrack(string name)
    {
        foreach (MusicTrack track in musicTracks)
        {
            if (track.name == name) return track;
        }
        return null;
    }
    #endregion



    IEnumerator StartAudioLoop(MusicTrack track) {
        for (float i = 0; i < track.intro.length; i+=Time.deltaTime) {
            yield return null;
        }
        
        audioSource.clip = track.loop;
        audioSource.loop = true;
        startingTrack = false;
        audioSource.Play();
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
