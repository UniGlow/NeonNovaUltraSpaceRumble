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
    List<ScoreCategoryResult> scoreCategoryResults = new List<ScoreCategoryResult>();



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

    public List<ScoreCategoryResult> GetScore()
    {
        StopTimer(Time.timeSinceLevelLoad);

        List<ScoreCategoryResult> scores = new List<ScoreCategoryResult>();

        if (activeTime != 0)
        {
            // hero orb hits
            float orbHeroHitsPerSecond = orbHeroHits / activeTime;
            int heroHitScore = orbHeroHits <= orbHeroHitsCategory.worstValue ? Mathf.RoundToInt(orbHeroHitsPerSecond.Remap(orbHeroHitsCategory.worstValue, 0f, orbHeroHitsCategory.optimalValue, gameSettings.OptimalScorePerSecond / gameSettings.RunnerScoreCategories.Count)) : 0;
            scores.Add(new ScoreCategoryResult(orbHeroHitsCategory, heroHitScore));

            // boss orb hits
            float orbBossHitsPerSecond = orbBossHits / activeTime;
            int bossHitScore = Mathf.RoundToInt((orbBossHitsPerSecond / orbBossHitsCategory.optimalValue) * gameSettings.OptimalScorePerSecond / gameSettings.RunnerScoreCategories.Count);
            scores.Add(new ScoreCategoryResult(orbBossHitsCategory, bossHitScore));
        }
        else
        {
            scores.Add(new ScoreCategoryResult(orbHeroHitsCategory, 0));
            scores.Add(new ScoreCategoryResult(orbBossHitsCategory, 0));
        }

        scoreCategoryResults = scores;
        return scores;
    }
}
