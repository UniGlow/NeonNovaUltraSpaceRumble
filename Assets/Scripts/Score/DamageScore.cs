using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScore : IScore
{
    int damageScore;
    int critDamageScore;



    public void CritDamageDone(int amount)
    {
        critDamageScore += amount;
    }

    public void DamageDone(int amount)
    {
        damageScore += amount;
    }

    public int GetScore()
    {
        // TODO
        return damageScore + critDamageScore;
    }
}
