using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelScore : IScore
{
    DamageScore damageScore;
    TankScore tankScore;
    RunnerScore runnerScore;
    List<ScoreCategoryResult> scoreCategoryResults = new List<ScoreCategoryResult>();



    public DamageScore DamageScore { get { return damageScore; } }
    public TankScore TankScore { get { return tankScore; } }
    public RunnerScore RunnerScore { get { return runnerScore; } }



    public LevelScore (GameSettings gameSettings, Points points)
    {
        damageScore = new DamageScore(gameSettings, points);
        tankScore = new TankScore(gameSettings, points);
        runnerScore = new RunnerScore(gameSettings, points);
    }

    public List<ScoreCategoryResult> GetScore()
    {
        List<ScoreCategoryResult> scores = new List<ScoreCategoryResult>();

        List<ScoreCategoryResult> damageScores = damageScore.GetScore();
        foreach (ScoreCategoryResult damageScoreResults in damageScores)
        {
            scores.Add(new ScoreCategoryResult(damageScoreResults.scoreCategory, damageScoreResults.result));
        }

        List<ScoreCategoryResult> tankScores = tankScore.GetScore();
        foreach (ScoreCategoryResult tankScore in tankScores)
        {
            scores.Add(new ScoreCategoryResult(tankScore.scoreCategory, tankScore.result));
        }

        List<ScoreCategoryResult> runnerScores = runnerScore.GetScore();
        foreach (ScoreCategoryResult runnerScore in runnerScores)
        {
            scores.Add(new ScoreCategoryResult(runnerScore.scoreCategory, runnerScore.result));
        }

        scoreCategoryResults = scores;
        return scores;
    }
}
