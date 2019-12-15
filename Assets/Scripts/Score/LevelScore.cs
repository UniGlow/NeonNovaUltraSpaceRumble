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

    public Dictionary<ScoreCategory, int> GetScore()
    {
        Dictionary<ScoreCategory, int> scores = new Dictionary<ScoreCategory, int>();

        Dictionary<ScoreCategory, int> damageScores = damageScore.GetScore();
        foreach (KeyValuePair<ScoreCategory, int> damageScore in damageScores)
        {
            scores.Add(damageScore.Key, damageScore.Value);
        }

        Dictionary<ScoreCategory, int> tankScores = tankScore.GetScore();
        foreach (KeyValuePair<ScoreCategory, int> tankScore in tankScores)
        {
            scores.Add(tankScore.Key, tankScore.Value);
        }

        Dictionary<ScoreCategory, int> runnerScores = runnerScore.GetScore();
        foreach (KeyValuePair<ScoreCategory, int> runnerScore in runnerScores)
        {
            scores.Add(runnerScore.Key, runnerScore.Value);
        }

        return scores;
    }
}
