﻿using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Provides all Custom Events we are using
/// </summary>
public class GameEvents {

    // Initializing the Delegates for the game events
    public delegate void GameEvent();
    public delegate void GameEventLevelEnd(string winner);

    // Create references for our delegates
    // This event is triggered when the player is free to move around
    public static event GameEventLevelEnd LevelCompleted;
    public static event GameEvent LevelStarted;



    // Helper functions to start events from within other classes
    public static void StartLevelCompleted(string winner)
    {
        LevelCompleted(winner);
    }
    public static void StartLevelStarted()
    {
        LevelStarted();
    }
}
