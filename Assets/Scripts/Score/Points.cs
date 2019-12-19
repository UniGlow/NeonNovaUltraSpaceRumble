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

    int bossTotalPoints = 0; // total points including shielded damage
    int bossPointsNormal = 0;
    int bossPointsCritical = 0;
    int bossTotalPointsShielded = 0;

    // TODO: Reset all these Lists when starting a new Match!
    List<Faction> winningFactions = new List<Faction>();

    List<int> bossDamageInLevels = new List<int>();
    List<int> bossCritDamageInLevels = new List<int>();
    List<int> bossDamageShieldedInLevels = new List<int>();
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
    public int BossTotalPoints { get { return bossTotalPoints; } }
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
    #endregion



    #region Unity Event Functions
	private void OnEnable () 
	{
        ResetPoints(false);
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
            RaiseLevelCompleted(Faction.Heroes);
            SaveBossPoints();
            currentHeroesPointLead = 0;
        }
        else if (-currentHeroesPointLead >= pointLeadToWin)
        {
            winningFactions.Add(Faction.Boss);
            RaiseLevelCompleted(Faction.Boss);
            SaveBossPoints();
            currentHeroesPointLead = 0;
        }
    }

    public void ResetPoints(bool endlessHealth)
    {
        currentHeroesPointLead = 0;
        bossTotalPoints = 0;
        bossTotalPointsShielded = 0;

        bossPointsNormal = 0;
        bossPointsCritical = 0;
        bossTotalPointsShielded = 0;

        this.endlessHealth = endlessHealth;
    }

    public void UpdateBossPoints(int amount, bool shielded, bool crit = false)
    {
        bossTotalPoints += amount;
        if (shielded) bossTotalPointsShielded += amount;
        if (crit) bossPointsCritical += amount;
        else bossPointsNormal += amount;

    }
    #endregion



    #region Private Functions
    private void SaveBossPoints()
    {
        bossDamageInLevels.Add(bossPointsNormal);
        bossCritDamageInLevels.Add(bossPointsCritical);
        bossDamageShieldedInLevels.Add(bossTotalPointsShielded);
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

