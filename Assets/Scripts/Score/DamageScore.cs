using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageScore : ClassScore, IScore
{
    int damageDone = 0;
    int critDamageDone = 0;

    float activeCritTime = 0f;

    bool crit = false;

    ScoreCategory damageDoneCategory;
    ScoreCategory critDamageDoneCategory;

    public int Damage { get { return damageDone; } }
    public int CritDamage { get { return critDamageDone; } }

    public DamageScore(GameSettings gameSettings, Points points) : base (gameSettings, points) 
    {
        damageDoneCategory = gameSettings.DamageScoreCategories.Find(x => x.name == "DamageDone");
        critDamageDoneCategory = gameSettings.DamageScoreCategories.Find(x => x.name == "CritDamageDone");
    }



    public void CritDamageDone(int amount)
    {
        critDamageDone += amount;
    }

    public void DamageDone(int amount)
    {
        damageDone += amount;
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


    public Dictionary<ScoreCategory, int> GetScore()
    {
        Dictionary<ScoreCategory, int> scores = new Dictionary<ScoreCategory, int>();

        // crit damage score
        float critDamageDonePerSecond = critDamageDone / activeCritTime;
        int critDamageScore = Mathf.RoundToInt((critDamageDonePerSecond / critDamageDoneCategory.optimalValue) * gameSettings.OptimalScorePerSecond);
        scores.Add(critDamageDoneCategory, critDamageScore);

        // normal damage score
        float damageDonePerSecond = damageDone / activeTime;
        int damageScore = Mathf.RoundToInt((damageDonePerSecond / damageDoneCategory.optimalValue) * gameSettings.OptimalScorePerSecond);
        scores.Add(damageDoneCategory, damageScore);

        return scores;
    }
}
