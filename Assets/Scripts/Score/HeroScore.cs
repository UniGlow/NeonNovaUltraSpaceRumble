using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Score/HeroScore")]
public class HeroScore : ScriptableObject, IScore 
{
    [SerializeField] PlayerConfig bossConfig = null;
    [SerializeField] GameSettings gameSettings = null;
    [SerializeField] Points points = null;

    List<LevelScore> levelScores = new List<LevelScore>();
    LevelScore currentLevelScore = null;
    List<ScoreCategoryResult> scoreCategoryResults = new List<ScoreCategoryResult>();



    public LevelScore CurrentLevelScore { get { return currentLevelScore; } }
    public List<LevelScore> LevelScores { get { return levelScores; } }


    private void OnEnable()
    {
        ClearAllLevels();
    }

    public void CreateLevelScore()
    {
        currentLevelScore = new LevelScore(gameSettings, points);
        levelScores.Add(currentLevelScore);
    }

    // TODO: Alfred needs to call this
    public void ClearAllLevels()
    {
        levelScores.Clear();
    }

    public void UpdateClassScoreStates(float timeStamp, PlayerColor heroColor, Ability.AbilityClass heroAbility)
    {
        switch (heroAbility)
        {
            case Ability.AbilityClass.Damage:
                if (bossConfig.ColorConfig == heroColor)
                    CurrentLevelScore.DamageScore.StartTimer(timeStamp, true);
                else
                    CurrentLevelScore.DamageScore.StartTimer(timeStamp, false);
                CurrentLevelScore.TankScore.StopTimer(timeStamp);
                CurrentLevelScore.RunnerScore.StopTimer(timeStamp);
                break;
            case Ability.AbilityClass.Runner:
                CurrentLevelScore.RunnerScore.StartTimer(timeStamp);
                CurrentLevelScore.DamageScore.StopTimer(timeStamp);
                CurrentLevelScore.TankScore.StopTimer(timeStamp);
                break;
            case Ability.AbilityClass.Tank:
                CurrentLevelScore.TankScore.StartTimer(timeStamp);
                CurrentLevelScore.DamageScore.StopTimer(timeStamp);
                CurrentLevelScore.RunnerScore.StopTimer(timeStamp);
                break;
        }
    }

    public void StopScoring(float timeStamp)
    {
        CurrentLevelScore.TankScore.StopTimer(timeStamp);
        CurrentLevelScore.DamageScore.StopTimer(timeStamp);
        CurrentLevelScore.RunnerScore.StopTimer(timeStamp);
    }

    public List<ScoreCategoryResult> GetScore()
    {
        List<ScoreCategoryResult> scores = new List<ScoreCategoryResult>();

        List<List<ScoreCategoryResult>> levelScoresResults = new List<List<ScoreCategoryResult>>();

        foreach (LevelScore levelScore in levelScores)
        {
            levelScoresResults.Add(levelScore.GetScore());
        }

        // merge all level scores
        foreach (List<ScoreCategoryResult> levelScore in levelScoresResults)
        {
            foreach (ScoreCategoryResult scoreCategoryResult in levelScore)
            {
                if (!scores.Contains(scoreCategoryResult)) scores.Add(new ScoreCategoryResult(scoreCategoryResult.scoreCategory, scoreCategoryResult.result));
                else scores.Find(x => x == scoreCategoryResult).result += scoreCategoryResult.result;
            }
        }

        scoreCategoryResults = scores;
        return scores;
    }
}
