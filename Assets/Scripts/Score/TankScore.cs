using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TankScore : ClassScore, IScore
{
    int damageShielded = 0;
    int bossTotalPointsDuringActivation = 0;
    float shieldedPercentage = 0f;
    int bossTotalPointsOnLastStart = 0;
    ScoreCategory damageShieldedCategory;
    List<ScoreCategoryResult> scoreCategoryResults = new List<ScoreCategoryResult>();



    public int Shielded { get { return damageShielded; } }
    public int BossTotalPointsDuringActivation { get { return bossTotalPointsDuringActivation; } }
    public float ShieldedPercentage { get { return shieldedPercentage; } }
    
    public TankScore(GameSettings gameSettings, Points points) : base (gameSettings, points)
    {
        damageShieldedCategory = gameSettings.TankScoreCategories.Find(x => x.name == "DamageShielded");
    }


    
    public List<ScoreCategoryResult> GetScore()
    {
        StopTimer(Time.timeSinceLevelLoad);

        List<ScoreCategoryResult> scores = new List<ScoreCategoryResult>();

        float score = 0;
        if(shieldedPercentage != 0)
            score = (shieldedPercentage / damageShieldedCategory.optimalValue) * gameSettings.OptimalScorePerSecond;
        scores.Add(new ScoreCategoryResult(damageShieldedCategory, Mathf.RoundToInt(score)));

        scoreCategoryResults = scores;
        return scores;
    }

    public override void StartTimer(float timeStamp, bool isBossWeaknessColor = false)
    {
        base.StartTimer(timeStamp, isBossWeaknessColor);

        bossTotalPointsOnLastStart = points.BossTotalPointsInLevel;
    }

    public override void StopTimer(float timeStamp)
    {
        CalculateShieldedPercentage();
        base.StopTimer(timeStamp);
    }

    public void DamageShielded(int amount)
    {
        damageShielded += amount;
    }



    void CalculateShieldedPercentage()
    {
        bossTotalPointsDuringActivation += points.BossTotalPointsInLevel - bossTotalPointsOnLastStart;
        
        if (bossTotalPointsDuringActivation == 0) return;

        shieldedPercentage = (float) damageShielded / (float) bossTotalPointsDuringActivation;
    }
}
