using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScore : IScore
{
    DamageScore damageScore = new DamageScore();
    TankScore tankScore = new TankScore();
    RunnerScore runnerScore = new RunnerScore();

    public DamageScore DamageScore { get { return damageScore; } }
    public TankScore TankScore { get { return tankScore; } }
    public RunnerScore RunnerScore { get { return runnerScore; } }



    public int GetScore()
    {
        throw new System.NotImplementedException();
    }
}
