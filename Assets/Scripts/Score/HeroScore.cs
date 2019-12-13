using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Score/HeroScore")]
public class HeroScore : ScriptableObject, IScore 
{
    LinkedList<LevelScore> levelScores = new LinkedList<LevelScore>();

    public LevelScore CurrentLevelScore { get { return levelScores.Last.Value; } }

    public void UpdateClassScoreStates(float timeStamp, PlayerColor bossColor, PlayerColor heroColor, Ability.AbilityClass heroAbility)
    {
        switch (heroAbility)
        {
            case Ability.AbilityClass.Damage:
                if (bossColor == heroColor)
                    CurrentLevelScore.DamageScore.StartCritDamage(timeStamp);
                else
                    CurrentLevelScore.DamageScore.StartNormalDamage(timeStamp);

                break;
        }
    }

    public int GetScore()
    {
        throw new System.NotImplementedException();
    }
}
