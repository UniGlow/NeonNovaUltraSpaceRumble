using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    #region Variable Declarations
    public static AudioManager Instance;

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
    [SerializeField] AudioSource audioSourceMusic = null;
    [SerializeField] AudioSource audioSourceSFX = null;

    bool startingTrack;
    #endregion



    #region Unity Event Functions
    void Awake()
    {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of an AudioManager.
            Debug.Log("There can only be one AudioManager instantiated. Destroying this Instance...");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSourceMusic = GetComponent<AudioSource>();
        audioSourceSFX = transform.GetChild(0).GetComponent<AudioSource>();
	}
	
	private void Update()
    {
		
	}
    #endregion



    #region Public Functions
    public void PlayWinSequence(Faction winner)
    {
        audioSourceMusic.Stop();
        audioSourceMusic.PlayOneShot(levelEnd, levelEndVolume);

        StartCoroutine(PlayWinSoundDelayed(winner));
    }

    public void StartTrack(string name)
    {
        if (startingTrack) return;
        startingTrack = true;

        MusicTrack track = GetTrack(name);
        audioSourceMusic.clip = track.intro;
        audioSourceMusic.volume = track.volume;
        audioSourceMusic.loop = false;
        audioSourceMusic.Play();
        StartCoroutine(StartAudioLoop(track));
    }

    public void StartTrack(MusicTrack track)
    {
        if (startingTrack) return;
        startingTrack = true;

        audioSourceMusic.clip = track.intro;
        audioSourceMusic.volume = track.volume;
        audioSourceMusic.loop = false;
        audioSourceMusic.Play();
        StartCoroutine(StartAudioLoop(track));
    }

    public void StartTutorialTrack()
    {
        if (startingTrack) return;
        startingTrack = true;

        MusicTrack track = GetTrack("TutorialTrack");
        audioSourceMusic.clip = track.intro;
        audioSourceMusic.volume = track.volume;
        audioSourceMusic.loop = false;
        audioSourceMusic.Play();
        StartCoroutine(StartAudioLoop(track));
    }

    public void StartRandomTrack()
    {
        if (startingTrack) return;
        startingTrack = true;

        // Ignore OriginalTrack and TutorialTrack
        MusicTrack track = musicTracks[Random.Range(2, musicTracks.Count)];
        audioSourceMusic.clip = track.intro;
        audioSourceMusic.volume = track.volume;
        audioSourceMusic.loop = false;
        audioSourceMusic.Play();
        StartCoroutine(StartAudioLoop(track));

        GameObject.FindObjectOfType<SongTextUpdater>().ShowSongTitle(track.artist, track.title);
    }

    public void StopPlaying() {
        audioSourceMusic.Stop();
        audioSourceMusic.clip = null;
        audioSourceMusic.volume = 1f;
        audioSourceMusic.loop = false;
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



    IEnumerator StartAudioLoop(MusicTrack track)
    {
        for (float i = 0; i < track.intro.length; i+=Time.deltaTime)
        {
            yield return null;
        }
        
        audioSourceMusic.clip = track.loop;
        audioSourceMusic.loop = true;
        startingTrack = false;
        audioSourceMusic.Play();
    }

    IEnumerator PlayWinSoundDelayed(Faction winner)
    {
        yield return new WaitForSecondsRealtime(levelEnd.length);

        audioSourceMusic.loop = false;
        if (winner == Faction.Heroes)
        {
            audioSourceMusic.clip = heroesWinSound;
            audioSourceMusic.volume = heroesWinSoundVolume;
        }
        else if (winner == Faction.Boss)
        {
            audioSourceMusic.clip = bossWinSound;
            audioSourceMusic.volume = bossWinSoundVolume;
        }

        audioSourceMusic.Play();
    }
}
