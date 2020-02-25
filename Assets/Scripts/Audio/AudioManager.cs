using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

/// <summary>
/// 
/// </summary>
public class AudioManager : MonoBehaviour
{

    #region Variable Declarations
    public static AudioManager Instance;

    [Header("Music Tracks")]
    [SerializeField] List<MusicTrack> musicTracks = new List<MusicTrack>();
    [SerializeField] MusicTrack tutorialTrack = null;
    [SerializeField] MusicTrack titleTrack = null;

    [Header("Sound Effects")]
    [SerializeField] AudioClip levelEnd = null;
    [Range(0,1)]
    [SerializeField] float levelEndVolume = 1f;
    [SerializeField] AudioClip bossWinSound = null;
    [Range(0, 1)]
    [SerializeField] float bossWinSoundVolume = 1f;
    [SerializeField] AudioClip heroesWinSound = null;
    [Range(0, 1)]
    [SerializeField] float heroesWinSoundVolume = 1f;
    [SerializeField] AudioClip slowMotion = null;
    [Range(0, 1)]
    [SerializeField] float slowMotionSoundVolume = 1f;
    [SerializeField] AudioClip menuConfirm = null;
    [Range(0, 1)]
    [SerializeField] float menuConfirmVolume = 1f;
    [SerializeField] AudioClip menuCancel = null;
    [Range(0, 1)]
    [SerializeField] float menuCancelVolume = 1f;

    [Header("References")]
    [SerializeField] AudioSource audioSourceMusic = null;
    [SerializeField] AudioSource audioSourceSFX = null;
    [Space]
    [SerializeField] GameEvent songChangedEvent = null;
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
            Destroy(gameObject);
        }
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

        RaiseSongChanged(track.artist, track.title);
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

        RaiseSongChanged(track.artist, track.title);
    }

    public void StartTitleTrack()
    {
        //if (startingTrack) return;
        if (audioSourceMusic.clip == titleTrack.intro || audioSourceMusic.clip == titleTrack.loop) return;
        startingTrack = true;

        audioSourceMusic.clip = titleTrack.intro;
        audioSourceMusic.volume = titleTrack.volume;
        audioSourceMusic.loop = false;
        audioSourceMusic.Play();
        StartCoroutine(StartAudioLoop(titleTrack));

        RaiseSongChanged(titleTrack.artist, titleTrack.title);
    }

    public void StartTutorialTrack()
    {
        if (startingTrack) return;
        startingTrack = true;

        audioSourceMusic.clip = tutorialTrack.intro;
        audioSourceMusic.volume = tutorialTrack.volume;
        audioSourceMusic.loop = false;
        audioSourceMusic.Play();
        StartCoroutine(StartAudioLoop(tutorialTrack));

        RaiseSongChanged(tutorialTrack.artist, tutorialTrack.title);
    }

    public void StartRandomTrack()
    {
        if (startingTrack) return;
        startingTrack = true;

        MusicTrack track = musicTracks[Random.Range(0, musicTracks.Count)];
        audioSourceMusic.clip = track.intro;
        audioSourceMusic.volume = track.volume;
        audioSourceMusic.loop = false;
        audioSourceMusic.Play();
        StartCoroutine(StartAudioLoop(track));

        RaiseSongChanged(track.artist, track.title);
    }

    public void StopPlaying()
    {
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

    public void BendTime(float targetValue, float duration, System.Action onComplete = null)
    {
        // Special to the usage of BendTime() in this project: The sound effect is for bending time in and out in one track
        if (targetValue < 1f) audioSourceSFX.PlayOneShot(slowMotion, slowMotionSoundVolume);

        audioSourceMusic.DOPitch(targetValue, duration).OnComplete(() =>
        {
            if (onComplete != null) onComplete.Invoke();
        });
    }

    public void PlayMenuConfirm()
    {
        audioSourceSFX.PlayOneShot(menuConfirm, menuConfirmVolume);
    }

    public void PlayMenuCancel()
    {
        audioSourceSFX.PlayOneShot(menuCancel, menuCancelVolume);
    }

    public void PauseMusic(bool pause)
    {
        if (pause)
            audioSourceMusic.Pause();
        else
            audioSourceMusic.UnPause();
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

    void RaiseSongChanged(string artist, string title)
    {
        songChangedEvent.Raise(this, artist, title);
    }



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
