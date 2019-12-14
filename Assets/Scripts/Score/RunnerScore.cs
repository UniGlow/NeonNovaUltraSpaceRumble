using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunnerScore : ClassScore, IScore
{
    int orbBossHits = 0;
    int orbHeroHits = 0;

    ScoreCategory orbBossHitsCategory;
    ScoreCategory orbHeroHitsCategory;


    public RunnerScore(GameSettings gameSettings, Points points) : base (gameSettings, points) 
    {
        orbBossHitsCategory = gameSettings.RunnerScoreCategories.Find(x => x.name == "OrbBossHits");
        orbHeroHitsCategory = gameSettings.RunnerScoreCategories.Find(x => x.name == "OrbHeroHits");
    }



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

        // hero orb hits
        float orbHeroHitsPerSecond = orbHeroHits / activeTime;
        scores.Add(orbHeroHitsCategory.name, Mathf.RoundToInt((orbHeroHitsPerSecond / orbHeroHitsCategory.optimalValue) * gameSettings.OptimalScorePerSecond));

        // boss orb hits
        float orbBossHitsPerSecond = orbBossHits / activeTime;
        scores.Add(orbBossHitsCategory.name, Mathf.RoundToInt((orbBossHitsPerSecond / orbBossHitsCategory.optimalValue) * gameSettings.OptimalScorePerSecond));

        return scores;
    }
}
