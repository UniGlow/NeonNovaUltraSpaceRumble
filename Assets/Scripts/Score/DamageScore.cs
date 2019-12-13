using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScore : IScore
{
    int damageScore;
    int critDamageScore;

    float normalDamageTime = 0f;
    float critDamageTime = 0f;

    float currentTimeStamp = -1f;

    bool crit = false;


    public void CritDamageDone(int amount)
    {
        critDamageScore += amount;
    }

    public void DamageDone(int amount)
    {
        damageScore += amount;
    }

    public void StartNormalDamage(float timeStamp)
    {
        if(currentTimeStamp == -1f)
        {
            currentTimeStamp = timeStamp;
        }
        else
        {
            if (crit)
                critDamageTime += timeStamp - currentTimeStamp;
            else
                Debug.LogWarning("Something went wrong!");
        }
        crit = false;
    }

    public void StartCritDamage(float timeStamp)
    {
        if(currentTimeStamp == -1f)
        {
            currentTimeStamp = timeStamp;
        }
        else
        {
            if (crit)
                normalDamageTime += timeStamp - currentTimeStamp;
            else
                Debug.LogWarning("Something went wrong!");
        }
        crit = true;
    }

    public void StopTimer(float timeStamp)
    {
        if (currentTimeStamp == -1f)
            return;
        if (crit)
        {
            critDamageTime += timeStamp - currentTimeStamp;
        }
        else
        {
            normalDamageTime += timeStamp - currentTimeStamp;
        }
        crit = false;
        currentTimeStamp = -1f;
    }


    public int GetScore()
    {
        // TODO
        return damageScore + critDamageScore;
    }
}
