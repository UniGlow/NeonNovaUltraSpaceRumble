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

    public Dictionary<ScoreCategory, int> GetScore()
    {
        Dictionary<ScoreCategory, int> scores = new Dictionary<ScoreCategory, int>();

        List<Dictionary<ScoreCategory, int>> levelScoresResults = new List<Dictionary<ScoreCategory, int>>();

        foreach (LevelScore levelScore in levelScores)
        {
            levelScoresResults.Add(levelScore.GetScore());
        }

        // merge all level scores
        foreach (Dictionary<ScoreCategory, int> levelScore in levelScoresResults)
        {
            foreach (KeyValuePair<ScoreCategory, int> scoreCategory in levelScore)
            {
                if (!scores.ContainsKey(scoreCategory.Key)) scores.Add(scoreCategory.Key, scoreCategory.Value);
                else scores[scoreCategory.Key] += scoreCategory.Value;
            }
        }

        return scores;
    }
}
