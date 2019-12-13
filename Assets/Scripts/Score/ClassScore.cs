﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClassScore
{
    protected float activeTime = 0f;
    protected float lastTimeStamp = -1f; // -1 if scoring not currently active
    protected GameSettings gameSettings;
    protected Points points;
    protected List<ScoreCategory> scoreCategories = new List<ScoreCategory>();



    public ClassScore(GameSettings gameSettings, Points points, List<ScoreCategory> scoreCategories)
    {
        this.gameSettings = gameSettings;
        this.points = points;
        this.scoreCategories = scoreCategories;
    }

    public virtual void Initialize(GameSettings gameSettings, Points points)
    {
        this.gameSettings = gameSettings;
        this.points = points;
    }

    public virtual void StartTimer(float timeStamp, bool isBossWeaknessColor = false)
    {
        if (lastTimeStamp == -1f)
        {
            lastTimeStamp = timeStamp;
        }
        else
        {
            Debug.LogWarning("Something went wrong!");
        }
    }

    public virtual void StopTimer(float timeStamp)
    {
        if (lastTimeStamp == -1f)
            return;
        activeTime += timeStamp - lastTimeStamp;
        lastTimeStamp = -1f;
    }
}