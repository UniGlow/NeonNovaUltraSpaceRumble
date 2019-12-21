using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Score/Points")]
public class Points : ScriptableObject 
{

    #region Variable Declarations
    // Public
    [Header("Settings")]
    [Tooltip("Current point lead to win. Is affected during runtime by the intensifyTimer of the GameplayManager.")]
    [SerializeField] int pointLeadToWin = 500;
    [SerializeField] bool endlessHealth = false;

    [Header("Game Events")]
    [SerializeField] GameEvent levelCompletedEvent = null;
    [SerializeField] GameEvent damageTakenEvent = null;

    // Private
    int currentHeroesPointLead = 0;

    int bossTotalPointsInLevel = 0; // total points including shielded damage
    int bossPointsNormal = 0;
    int bossPointsCritical = 0;
    int bossTotalPointsShielded = 0;

    List<Faction> winningFactions = new List<Faction>();

    List<int> bossDamageInLevels = new List<int>();
    List<int> bossCritDamageInLevels = new List<int>();
    List<int> bossDamageShieldedInLevels = new List<int>();

    List<float> levelTimes = new List<float>();
    int pointsForLeadingHero;
    #endregion



    #region Public Properties
    public int PointLeadToWin { get { return pointLeadToWin; }
        set
        {
            // Make sure the current points scale according to the new point lead goal
            float scalingFactor = (float) value / (float) pointLeadToWin;
            currentHeroesPointLead = Mathf.RoundToInt(currentHeroesPointLead * scalingFactor);

            pointLeadToWin = value;
        }
    }
    public int BossTotalPointsInLevel { get { return bossTotalPointsInLevel; } }
    public List<Faction> WinningFactions { get { return winningFactions; } }
    public int BossWins
    {
        get
        {
            int wins = 0;
            foreach (Faction faction in winningFactions)
            {
                if (faction == Faction.Boss) wins++;
            }
            return wins;
        }
    }
    public int HeroWins
    {
        get
        {
            int wins = 0;
            foreach (Faction faction in winningFactions)
            {
                if (faction == Faction.Heroes) wins++;
            }
            return wins;
        }
    }
    public List<int> BossDamageInLevels { get { return bossDamageInLevels; } }
    public List<int> BossCritDamageInLevels { get { return bossCritDamageInLevels; } }
    public List<int> BossDamageShieldedInLevels { get { return bossDamageShieldedInLevels; } }
    public List<float> LevelTimes { get { return levelTimes; } }
    public int PointsForLeadingHero { get { return pointsForLeadingHero; }
        set
        {
            if (value > pointsForLeadingHero) pointsForLeadingHero = value;
        }
    }
    #endregion



    #region Unity Event Functions
	private void OnEnable () 
	{
        ResetLevelPoints(false);
	}
    #endregion



    #region Public Functions
    public void ScorePoints(Faction scoringFaction, int amount)
    {
        switch (scoringFaction)
        {
            case Faction.Heroes:
                currentHeroesPointLead += amount;
                break;
            case Faction.Boss:
                currentHeroesPointLead -= amount;
                break;
            default:
                break;
        }

        if (currentHeroesPointLead >= 0)
            RaiseDamageTaken(Faction.Heroes, currentHeroesPointLead);

        if (currentHeroesPointLead < 0)
            RaiseDamageTaken(Faction.Boss, -currentHeroesPointLead);

        if (endlessHealth) return;

        // Did one faction win?
        if (currentHeroesPointLead >= pointLeadToWin)
        {
            winningFactions.Add(Faction.Heroes);
            SaveBossPoints();
            currentHeroesPointLead = 0;
            RaiseLevelCompleted(Faction.Heroes);
        }
        else if (-currentHeroesPointLead >= pointLeadToWin)
        {
            winningFactions.Add(Faction.Boss);
            SaveBossPoints();
            currentHeroesPointLead = 0;
            RaiseLevelCompleted(Faction.Boss);
        }
    }

    /// <summary>
    /// Resets the Data collected during this Level.
    /// </summary>
    /// <param name="endlessHealth"></param>
    public void ResetLevelPoints(bool endlessHealth)
    {
        currentHeroesPointLead = 0;
        bossTotalPointsInLevel = 0;
        bossTotalPointsShielded = 0;

        bossPointsNormal = 0;
        bossPointsCritical = 0;
        bossTotalPointsShielded = 0;

        this.endlessHealth = endlessHealth;
    }

    public void UpdateBossPoints(int amount, bool shielded, bool crit = false)
    {
        bossTotalPointsInLevel += amount;
        if (shielded) bossTotalPointsShielded += amount;
        if (crit) bossPointsCritical += amount;
        else bossPointsNormal += amount;
    }

    /// <summary>
    /// Resets the Points Object and deletes all saved Data of the last Match.
    /// </summary>
    public void ResetPoints()
    {
        ResetLevelPoints(endlessHealth);
        winningFactions.Clear();
        bossDamageInLevels.Clear();
        bossCritDamageInLevels.Clear();
        bossDamageShieldedInLevels.Clear();
        levelTimes.Clear();
        pointsForLeadingHero = 0;
    }
    #endregion



    #region Private Functions
    private void SaveBossPoints()
    {
        bossDamageInLevels.Add(bossPointsNormal);
        bossCritDamageInLevels.Add(bossPointsCritical);
        bossDamageShieldedInLevels.Add(bossTotalPointsShielded);
        levelTimes.Add(Time.timeSinceLevelLoad - 4f);
    }
    #endregion



    #region GameEvent Raiser
    void RaiseLevelCompleted(Faction winner)
    {
        levelCompletedEvent.Raise(this, winner);
    }
    void RaiseDamageTaken(Faction leadingFaction, int pointLead)
    {
        damageTakenEvent.Raise(this, leadingFaction, pointLead);
    }
    #endregion



    #region Coroutines

    #endregion
}

