using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScore : IScore
{
    float tankTime = 0f;

    float currentTimeStamp = -1f;


    public void StartTimer(float timeStamp)
    {
        if(currentTimeStamp == -1f)
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
        tankTime += timeStamp - currentTimeStamp;
        currentTimeStamp = -1f;
    }

    public int GetScore()
    {
        throw new System.NotImplementedException();
    }
}
