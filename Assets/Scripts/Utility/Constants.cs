using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class containing all strings and constants used in the game
/// </summary>
public class Constants 
{
    #region Inputs
    public static readonly string INPUT_HORIZONTAL = "Horizontal";
    public static readonly string INPUT_VERTICAL = "Vertical";
    public static readonly string INPUT_LOOK_HORIZONTAL = "LookHorizontal";
    public static readonly string INPUT_LOOK_VERTICAL = "LookVertical";
    public static readonly string INPUT_ABILITY = "Ability";
    public static readonly string INPUT_TRANSMIT = "Transmit";
    public static readonly string INPUT_DEBUGMODE = "DebugMode";
    public static readonly string INPUT_ESCAPE = "Escape";
    public static readonly string INPUT_SUBMIT = "Submit";
    public static readonly string INPUT_CANCEL = "Cancel";
    #endregion

    #region Tags and Layers
    public static readonly string TAG_BOSS = "Boss";
    public static readonly string TAG_HERO = "Hero";
    public static readonly string TAG_WALL = "Wall";
    public static readonly string TAG_SHIELD = "Shield";
    public static readonly string TAG_HOMING_MISSILE = "HomingMissile";
    public static readonly string TAG_AI_CORNER = "Corner";
    public static readonly string TAG_AI_MIDDLE = "AIMiddle";
    #endregion

    #region Scenes
    public static readonly string SCENE_OPTIONS = "OptionsMenu";
    public static readonly string SCENE_MAIN_MENU = "MainMenu";
    #endregion

    #region Sounds
    // Exposed Parameters in Mixers
    public static readonly string MIXER_SFX_VOLUME = "SFXVolume";
    public static readonly string MIXER_MUSIC_VOLUME = "MusicVolume";
    #endregion
}