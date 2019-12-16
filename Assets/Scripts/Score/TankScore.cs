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

    public int Shielded { get { return damageShielded; } }
    public int BossTotalPointsDuringActivation { get { return bossTotalPointsDuringActivation; } }
    public float ShieldedPercentage { get { return shieldedPercentage; } }
    
    public TankScore(GameSettings gameSettings, Points points) : base (gameSettings, points)
    {
        damageShieldedCategory = gameSettings.TankScoreCategories.Find(x => x.name == "DamageShielded");
    }


    
    public Dictionary<ScoreCategory, int> GetScore()
    {
        Dictionary<ScoreCategory, int> scores = new Dictionary<ScoreCategory, int>();

        // Calculate current shieldedPercentage only if scoring is currently running
        if (lastTimeStamp != -1f) CalculateShieldedPercentage();

        float score = (shieldedPercentage / damageShieldedCategory.optimalValue) * gameSettings.OptimalScorePerSecond;

        scores.Add(damageShieldedCategory, Mathf.RoundToInt(score));

        return scores;
    }

    public override void StartTimer(float timeStamp, bool isBossWeaknessColor = false)
    {
        base.StartTimer(timeStamp, isBossWeaknessColor);

        bossTotalPointsOnLastStart = points.BossTotalPoints;
    }

    public override void StopTimer(float timeStamp)
    {
        base.StopTimer(timeStamp);

        CalculateShieldedPercentage();
    }

    public void DamageShielded(int amount)
    {
        damageShielded += amount;
    }



    void CalculateShieldedPercentage()
    {
        bossTotalPointsDuringActivation += points.BossTotalPoints - bossTotalPointsOnLastStart;
        
        if (bossTotalPointsDuringActivation == 0) return;

        shieldedPercentage = (float) damageShielded / (float) bossTotalPointsDuringActivation;
    }
}
