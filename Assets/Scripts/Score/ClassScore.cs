using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClassScore
{
    protected float activeTime = 0f;
    protected float currentTimeStamp = -1f;
    protected GameSettings gameSettings;
    protected Points points;



    public ClassScore(GameSettings gameSettings, Points points)
    {
        this.gameSettings = gameSettings;
        this.points = points;
    }

    public virtual void Initialize(GameSettings gameSettings, Points points)
    {
        this.gameSettings = gameSettings;
        this.points = points;
    }

    public virtual void StartTimer(float timeStamp, bool isBossWeaknessColor = false)
    {
        if (currentTimeStamp == -1f)
        {
            currentTimeStamp = timeStamp;
        }
        else
        {
            Debug.LogWarning("Something went wrong!");
        }
    }

    public virtual void StopTimer(float timeStamp)
    {
        if (currentTimeStamp == -1f)
            return;
        activeTime += timeStamp - currentTimeStamp;
        currentTimeStamp = -1f;
    }
}
