using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(fileName = "New Music Track", menuName = "Scriptable Objects/Music Track")]
public class MusicTrack : ScriptableObject 
{
    public AudioClip intro;
    public AudioClip loop;
    [Range(0f,1f)]
    public float volume = 0.8f;
    public string artist;
    public string title;
}
