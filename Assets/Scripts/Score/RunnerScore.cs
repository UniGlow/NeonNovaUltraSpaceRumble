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

    public int OrbBossHits { get { return orbBossHits; } }
    public int OrbHeroHits { get { return orbHeroHits; } }

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

    public Dictionary<ScoreCategory, int> GetScore()
    {
        StopTimer(Time.timeSinceLevelLoad);

        Dictionary<ScoreCategory, int> scores = new Dictionary<ScoreCategory, int>();

        if (activeTime != 0)
        {
            // hero orb hits
            float orbHeroHitsPerSecond = orbHeroHits / activeTime;
            int heroHitScore = orbHeroHits <= orbHeroHitsCategory.worstValue ? Mathf.RoundToInt(orbHeroHitsPerSecond.Remap(orbHeroHitsCategory.worstValue, 0f, orbHeroHitsCategory.optimalValue, gameSettings.OptimalScorePerSecond / gameSettings.RunnerScoreCategories.Count)) : 0;
            scores.Add(orbHeroHitsCategory, heroHitScore);

            // boss orb hits
            float orbBossHitsPerSecond = orbBossHits / activeTime;
            int bossHitScore = Mathf.RoundToInt((orbBossHitsPerSecond / orbBossHitsCategory.optimalValue) * gameSettings.OptimalScorePerSecond / gameSettings.RunnerScoreCategories.Count);
            scores.Add(orbBossHitsCategory, bossHitScore);
        }
        else
        {
            scores.Add(orbHeroHitsCategory, 0);
            scores.Add(orbBossHitsCategory, 0);
        }

        return scores;
    }
}
