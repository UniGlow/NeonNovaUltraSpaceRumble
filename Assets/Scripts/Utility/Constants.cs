using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class containing all strings and constants used in the game
/// </summary>
public class Constants 
{
    #region Tags and Layers
    public static readonly string TAG_BOSS = "Boss";
    public static readonly string TAG_BOSS_DUMMY = "BossDummy";
    public static readonly string TAG_HERO = "Hero";
    public static readonly string TAG_HERO_DUMMY = "HeroDummy";
    public static readonly string TAG_WALL = "Wall";
    public static readonly string TAG_SHIELD = "Shield";
    public static readonly string TAG_HOMING_MISSILE = "HomingMissile";
    public static readonly string TAG_AI_CORNER = "Corner";
    public static readonly string TAG_AI_MIDDLE = "AIMiddle";
    public static readonly string TAG_SPAWN_POINT = "SpawnPoint";
    #endregion

    #region Scenes
    public static readonly string SCENE_UI_LEVEL = "UILevel";
    public static readonly string SCENE_UI_LOBBY = "UILobby";
    public static readonly string SCENE_UI_CREDITS = "UICredits";
    #endregion

    #region Sounds
    // Exposed Parameters in Mixers
    public static readonly string MIXER_SFX_VOLUME = "SFXVolume";
    public static readonly string MIXER_MUSIC_VOLUME = "MusicVolume";
    #endregion
}