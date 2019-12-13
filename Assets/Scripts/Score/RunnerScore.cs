using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunnerScore : ClassScore, IScore
{
    int orbBossHits = 0;
    int orbHeroHits = 0;


    public RunnerScore(GameSettings gameSettings, Points points, List<ScoreCategory> scoreCategories) : base(gameSettings, points, scoreCategories) { }



    public void RegisterOrbHit(Faction faction)
    {
        if (faction == Faction.Boss)
        {
            orbBossHits++;
        }
        else if (faction == Faction.Heroes)
        {
            orbHeroHits++;
        }
    }

    public Dictionary<string, int> GetScore()
    {
        Dictionary<string, int> scores = new Dictionary<string, int>();



        return scores;
    }
}
