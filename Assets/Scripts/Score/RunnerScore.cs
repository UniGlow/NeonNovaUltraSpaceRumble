using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunnerScore : ClassScore, IScore
{



    public RunnerScore(GameSettings gameSettings, Points points) : base(gameSettings, points) { }



    public int GetScore()
    {
        throw new System.NotImplementedException();
    }
}
