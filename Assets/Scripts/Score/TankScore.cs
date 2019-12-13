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


    
    public TankScore(GameSettings gameSettings, Points points, List<ScoreCategory> scoreCategories) : base(gameSettings, points, scoreCategories) { }


    
    public Dictionary<string, int> GetScore()
    {
        Dictionary<string, int> scores = new Dictionary<string, int>();

        // Calculate current shieldedPercentage only if scoring is currently running
        if (lastTimeStamp != -1f) CalculateShieldedPercentage();

        scores.Add(gameSettings.DamageShielded, Mathf.RoundToInt((shieldedPercentage / gameSettings.OptimalShieldPercentage) * gameSettings.OptimalScorePerSecond));

        return scores;
    }

    public override void StartTimer(float timeStamp, bool isBossWeaknessColor = false)
    {
        base.StartTimer(timeStamp, isBossWeaknessColor);

        bossTotalPointsOnLastStart = points.BossTotalPoints;
        damageShielded = 0;
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
        shieldedPercentage = damageShielded / bossTotalPointsDuringActivation;
    }
}
