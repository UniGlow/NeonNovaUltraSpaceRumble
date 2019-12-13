using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelScore : IScore
{
    DamageScore damageScore;
    TankScore tankScore;
    RunnerScore runnerScore;

    public DamageScore DamageScore { get { return damageScore; } }
    public TankScore TankScore { get { return tankScore; } }
    public RunnerScore RunnerScore { get { return runnerScore; } }



    public LevelScore (GameSettings gameSettings, Points points)
    {
        damageScore = new DamageScore(gameSettings, points);
        tankScore = new TankScore(gameSettings, points);
        runnerScore = new RunnerScore(gameSettings, points);
    }

    public int GetScore()
    {
        throw new System.NotImplementedException();
    }
}
