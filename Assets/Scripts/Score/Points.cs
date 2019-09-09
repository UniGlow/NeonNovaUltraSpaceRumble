using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Points")]
public class Points : ScriptableObject 
{

    #region Variable Declarations
    // Public
    [Header("Settings")]
    [SerializeField] int pointLeadToWin = 500;
    [SerializeField] bool endlessHealth = false;

    [Header("Game Events")]
    [SerializeField] GameEvent levelCompletedEvent = null;
    [SerializeField] GameEvent damageTakenEvent = null;

    // Private
    int currentHeroesPointLead = 0;
	#endregion
	
	
	
	#region Public Properties
	public int PointLeadToWin { get { return pointLeadToWin; }
        set
        {
            // Make sure the current points scale according to the new point lead goal
            currentHeroesPointLead = currentHeroesPointLead * (value / pointLeadToWin);

            pointLeadToWin = value;
        }
    }
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
            RaiseLevelCompleted(Faction.Heroes);
            currentHeroesPointLead = 0;
        }
        else if (-currentHeroesPointLead >= pointLeadToWin)
        {
            RaiseLevelCompleted(Faction.Boss);
            currentHeroesPointLead = 0;
        }
    }

    public void ResetPoints(bool endlessHealth)
    {
        currentHeroesPointLead = 0;
        this.endlessHealth = endlessHealth;
    }
    #endregion



    #region Private Functions

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

