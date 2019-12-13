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



    public LevelScore (GameSettings gameSettings, Points points, List<ScoreCategory> scoreCategories)
    {
        // TODO: scoreCategories need to be broken down into their respective categories
        damageScore = new DamageScore(gameSettings, points, scoreCategories);
        tankScore = new TankScore(gameSettings, points, scoreCategories);
        runnerScore = new RunnerScore(gameSettings, points, scoreCategories);
    }

    public Dictionary<string, int> GetScore()
    {
        throw new System.NotImplementedException();
    }
}
