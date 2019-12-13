using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageScore : ClassScore, IScore
{
    int damageScore;
    int critDamageScore;

    float activeCritTime = 0f;

    bool crit = false;



    public DamageScore(GameSettings gameSettings, Points points, List<ScoreCategory> scoreCategories) : base(gameSettings, points, scoreCategories) { }



    public void CritDamageDone(int amount)
    {
        critDamageScore += amount;
    }

    public void DamageDone(int amount)
    {
        damageScore += amount;
    }

    public override void StartTimer(float timeStamp, bool isBossWeaknessColor)
    {
        // Boss Color changed, player stays damage
        if (lastTimeStamp != -1f)
        {
            if (isBossWeaknessColor)
                activeCritTime += timeStamp - lastTimeStamp;
            else
                activeTime += timeStamp - lastTimeStamp;
        }

        lastTimeStamp = timeStamp;
        crit = isBossWeaknessColor;
    }

    public override void StopTimer(float timeStamp)
    {
        // Already stopped
        if (lastTimeStamp == -1f)
            return;

        if (crit)
        {
            activeCritTime += timeStamp - lastTimeStamp;
        }
        else
        {
            activeTime += timeStamp - lastTimeStamp;
        }

        lastTimeStamp = -1f;
    }


    public Dictionary<string, int> GetScore()
    {
        Dictionary<string, int> scores = new Dictionary<string, int>();

        float fullActiveTime = activeTime + activeCritTime;
        float percentageCritTime = activeCritTime / fullActiveTime;
        float optimalDamagePerSecond = (gameSettings.OptimalDamageDealt * percentageCritTime + (gameSettings.OptimalDamageDealt / 2) * (1 - percentageCritTime)) * fullActiveTime;
        float damagePerSecond = (damageScore + critDamageScore) / fullActiveTime;

        // TODO: Split up categories returned here into normal and crit damage
        scores.Add("Combined Damage dealt (normal+crit)", Mathf.RoundToInt((damagePerSecond / optimalDamagePerSecond) * gameSettings.OptimalScorePerSecond * fullActiveTime));

        return scores;
    }
}
