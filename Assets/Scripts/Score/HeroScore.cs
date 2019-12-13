using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Score/HeroScore")]
public class HeroScore : ScriptableObject, IScore 
{
    LinkedList<LevelScore> levelScores = new LinkedList<LevelScore>();
    [SerializeField] PlayerConfig bossConfig = null;

    public LevelScore CurrentLevelScore { get { return levelScores.Last.Value; } }

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

    public int GetScore()
    {
        throw new System.NotImplementedException();
    }
}
