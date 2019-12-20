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
        Dictionary<ScoreCategory, int> scores = new Dictionary<ScoreCategory, int>();

        // hero orb hits
        float orbHeroHitsPerSecond = orbHeroHits / activeTime;
        int heroHitScore = 0;
        if (orbHeroHits != 0)
        {
            // TODO: Remap-Function may not support inverted Remapping from(0.3 - 0) to(0 - 100)
            heroHitScore = Mathf.RoundToInt(orbHeroHitsPerSecond.Remap(orbHeroHitsCategory.worstValue, 0f, orbHeroHitsCategory.optimalValue, gameSettings.OptimalScorePerSecond));
        }
        scores.Add(orbHeroHitsCategory, heroHitScore);

        // boss orb hits
        float orbBossHitsPerSecond = orbBossHits / activeTime;
        int bossHitScore = 0;
        if (orbBossHits != 0)
            bossHitScore = Mathf.RoundToInt((orbBossHitsPerSecond / orbBossHitsCategory.optimalValue) * gameSettings.OptimalScorePerSecond);
        scores.Add(orbBossHitsCategory, bossHitScore);

        return scores;
    }
}
