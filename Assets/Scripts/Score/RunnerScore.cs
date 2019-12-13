using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerScore : IScore
{
    float runnerTime = 0f;

    float currentTimeStamp = -1f;


    public void StartTimer(float timeStamp)
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

    public void StopTimer(float timeStamp)
    {
        if (currentTimeStamp == -1f)
            return;
        runnerTime += timeStamp - currentTimeStamp;
        currentTimeStamp = -1f;
    }

    public int GetScore()
    {
        throw new System.NotImplementedException();
    }
}
