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

    public Dictionary<string, int> GetScore()
    {
        Dictionary<string, int> scores = new Dictionary<string, int>();

        Dictionary<string, int> damageScores = damageScore.GetScore();
        foreach (KeyValuePair<string, int> damageScore in damageScores)
        {
            scores.Add(damageScore.Key, damageScore.Value);
        }

        Dictionary<string, int> tankScores = tankScore.GetScore();
        foreach (KeyValuePair<string, int> tankScore in tankScores)
        {
            scores.Add(tankScore.Key, tankScore.Value);
        }

        Dictionary<string, int> runnerScores = runnerScore.GetScore();
        foreach (KeyValuePair<string, int> runnerScore in runnerScores)
        {
            scores.Add(runnerScore.Key, runnerScore.Value);
        }

        return scores;
    }
}
