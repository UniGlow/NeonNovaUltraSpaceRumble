using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class TutorialPlayerManager : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("Player Configs")]
    [SerializeField] private PlayerConfig bossPlayerConfig = null;
    [SerializeField] private PlayerConfig hero1PlayerConfig = null;
    [SerializeField] private PlayerConfig hero2PlayerConfig = null;
    [SerializeField] private PlayerConfig hero3PlayerConfig = null;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject heroPrefab = null;
    [SerializeField] private GameObject bossPrefab = null;

    [Header("GameEvents")]
    [SerializeField] private GameEvent playersSpawnedEvent = null;

    [Header("References")]
    [SerializeField] private Points points = null;
    [SerializeField] private GameSettings gameSettings = null;

    // Private
    // PlayerTransforms
    private Transform player1Transform = null;
    private Transform player2Transform = null;
    private Transform player3Transform = null;
    private Transform bossTransform = null;

    private Hero player1 = null;
    private Hero player2 = null;
    private Hero player3 = null;
    private Boss boss = null;

    private int humanPlayerCount = 0;

    #endregion



    #region Public Properties
    public int HumanPlayerCount { get { return humanPlayerCount; } }
    #endregion



    #region Unity Event Functions

	#endregion
	
	
	
	#region Public Functions
	public void InitializePlayers()
    {
        if(player1 != null)
        {
            player1 = null;
            Destroy(player1Transform.gameObject);
            player1Transform = null;
        }
        if(player2 != null)
        {
            player2 = null;
            Destroy(player2Transform.gameObject);
            player2Transform = null;
        }
        if(player3 != null)
        {
            player3 = null;
            Destroy(player3Transform.gameObject);
            player3Transform = null;
        }
        if(boss != null)
        {
            boss = null;
            Destroy(bossTransform.gameObject);
            bossTransform = null;
        }

        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag(Constants.TAG_SPAWN_POINT);
        foreach(GameObject go in spawnPointObjects)
        {
            if (go.GetComponent<SpawnPoint>() != null)
            {
                if (go.GetComponent<SpawnPoint>().objectToSpawn == SpawnPoint.SpawnObject.Hero1)
                {
                    player1Transform = Instantiate(heroPrefab, go.transform.position, go.transform.rotation).transform;
                    humanPlayerCount++;

                    player1 = player1Transform.GetComponent<Hero>();
                    player1.SetPlayerConfig(hero1PlayerConfig);
                    hero1PlayerConfig.playerTransform = player1Transform;
                }

                else if (go.GetComponent<SpawnPoint>().objectToSpawn == SpawnPoint.SpawnObject.Hero2)
                {
                    player2Transform = Instantiate(heroPrefab, go.transform.position, go.transform.rotation).transform;
                    humanPlayerCount++;

                    player2 = player2Transform.GetComponent<Hero>();
                    player2.SetPlayerConfig(hero2PlayerConfig);
                    hero2PlayerConfig.playerTransform = player2Transform;
                }

                else if (go.GetComponent<SpawnPoint>().objectToSpawn == SpawnPoint.SpawnObject.Hero3)
                {
                    player3Transform = Instantiate(heroPrefab, go.transform.position, go.transform.rotation).transform;
                    humanPlayerCount++;

                    player3 = player3Transform.GetComponent<Hero>();
                    player3.SetPlayerConfig(hero3PlayerConfig);
                    hero3PlayerConfig.playerTransform = player3Transform;
                }

                else if (go.GetComponent<SpawnPoint>().objectToSpawn == SpawnPoint.SpawnObject.Boss)
                {
                    bossTransform = Instantiate(bossPrefab, go.transform.position, go.transform.rotation).transform;
                    humanPlayerCount++;

                    boss = bossTransform.GetComponent<Boss>();
                    boss.SetPlayerConfig(bossPlayerConfig, GameManager.Instance.activeColorSet);
                    bossPlayerConfig.playerTransform = bossTransform;
                }
            }
        }

        switch (humanPlayerCount)
        {
            case 1:
                points.PointLeadToWin = gameSettings.BossWinningSolo;
                break;
            case 2:
                points.PointLeadToWin = gameSettings.BossWinningDuo;
                break;
            case 3:
                points.PointLeadToWin = gameSettings.BossWinningTriple;
                break;
            default:
                break;
        }

        RaisePlayersSpawned(hero1PlayerConfig, hero2PlayerConfig, hero3PlayerConfig, bossPlayerConfig);
    }
	#endregion
	
	
	
	#region Private Functions
    private void RaisePlayersSpawned(PlayerConfig hero1PlayerConfig, PlayerConfig hero2PlayerConfig, PlayerConfig hero3PlayerConfig, PlayerConfig bossPlayerConfig)
    {
        playersSpawnedEvent.Raise(this, hero1PlayerConfig, hero2PlayerConfig, hero3PlayerConfig, bossPlayerConfig);
    }
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

