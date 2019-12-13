using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageScore : ClassScore, IScore
{
    int damageScore;
    int critDamageScore;

    float activeCritTime = 0f;

    bool crit = false;



    public DamageScore(GameSettings gameSettings, Points points) : base(gameSettings, points) { }



    public void CritDamageDone(int amount)
    {
        critDamageScore += amount;
    }

    public void DamageDone(int amount)
    {
        damageScore += amount;
    }

    public override void StartTimer(float timeStamp, bool isBossWeaknessColor)
    {
        if (currentTimeStamp == -1f)
        {
            currentTimeStamp = timeStamp;
        }
        else
        {
            if (isBossWeaknessColor)
                activeCritTime += timeStamp - currentTimeStamp;
            else
                activeTime += timeStamp - currentTimeStamp;
        }
        crit = isBossWeaknessColor;
    }

    public override void StopTimer(float timeStamp)
    {
        if (currentTimeStamp == -1f)
            return;
        if (crit)
        {
            activeCritTime += timeStamp - currentTimeStamp;
        }
        else
        {
            activeTime += timeStamp - currentTimeStamp;
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
