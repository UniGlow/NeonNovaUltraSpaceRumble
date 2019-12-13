using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScore : ClassScore, IScore
{
    int damageShielded = 0;



    public override void StartTimer(float timeStamp, bool isBossWeaknessColor = false)
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
    
    public int GetScore()
    {
        throw new System.NotImplementedException();
    }

    public void DamageShielded(int amount)
    {
        damageShielded += amount;
    }
}
