using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TankScore : ClassScore, IScore
{
    int damageShielded = 0;


    
    public TankScore(GameSettings gameSettings, Points points) : base(gameSettings, points) { }


    
    public int GetScore()
    {
        throw new System.NotImplementedException();
    }

    public void DamageShielded(int amount)
    {
        damageShielded += amount;
    }
}
