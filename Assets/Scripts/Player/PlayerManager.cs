using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerManager : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("Player Configs")]
    [SerializeField] private PlayerConfig bossPlayerConfig = null;
    [SerializeField] private PlayerConfig hero1PlayerConfig = null;
    [SerializeField] private PlayerConfig hero2PlayerConfig = null;
    [SerializeField] private PlayerConfig hero3PlayerConfig = null;
    
    [Header("Prefabs to Spawn")]
    [SerializeField] private GameObject heroPrefab = null;
    [SerializeField] private GameObject heroAIPrefab = null;
    [SerializeField] private GameObject bossPrefab = null;
    [SerializeField] private GameObject bossAIPrefab = null;

    [Header("GameEvents")]
    [SerializeField] private GameEvent playersSpawnedEvent = null;

    [Header("References")]
    [SerializeField] private Points points = null;

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
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag(Constants.TAG_SPAWN_POINT);
        foreach(GameObject go in spawnPointObjects)
        {
            if (go.GetComponent<SpawnPoint>() != null)
            {
                if (go.GetComponent<SpawnPoint>().objectToSpawn == SpawnPoint.SpawnObject.Hero1)
                {
                    if (hero1PlayerConfig.AIControlled)
                        player1Transform = Instantiate(heroAIPrefab, go.transform.position, go.transform.rotation).transform;
                    else
                    {
                        player1Transform = Instantiate(heroPrefab, go.transform.position, go.transform.rotation).transform;
                        humanPlayerCount++;
                    }

                    player1 = player1Transform.GetComponent<Hero>();
                    player1.SetPlayerConfig(hero1PlayerConfig);
                    hero1PlayerConfig.playerTransform = player1Transform;
                }

                else if (go.GetComponent<SpawnPoint>().objectToSpawn == SpawnPoint.SpawnObject.Hero2)
                {
                    if (hero2PlayerConfig.AIControlled)
                        player2Transform = Instantiate(heroAIPrefab, go.transform.position, go.transform.rotation).transform;
                    else
                    {
                        player2Transform = Instantiate(heroPrefab, go.transform.position, go.transform.rotation).transform;
                        humanPlayerCount++;
                    }

                    player2 = player2Transform.GetComponent<Hero>();
                    player2.SetPlayerConfig(hero2PlayerConfig);
                    hero2PlayerConfig.playerTransform = player2Transform;
                }

                else if (go.GetComponent<SpawnPoint>().objectToSpawn == SpawnPoint.SpawnObject.Hero3)
                {
                    if (hero3PlayerConfig.AIControlled)
                        player3Transform = Instantiate(heroAIPrefab, go.transform.position, go.transform.rotation).transform;
                    else
                    {
                        player3Transform = Instantiate(heroPrefab, go.transform.position, go.transform.rotation).transform;
                        humanPlayerCount++;
                    }

                    player3 = player3Transform.GetComponent<Hero>();
                    player3.SetPlayerConfig(hero3PlayerConfig);
                    hero3PlayerConfig.playerTransform = player3Transform;
                }

                else if (go.GetComponent<SpawnPoint>().objectToSpawn == SpawnPoint.SpawnObject.Boss)
                {
                    if (bossPlayerConfig.AIControlled)
                        bossTransform = Instantiate(bossAIPrefab, go.transform.position, go.transform.rotation).transform;
                    else
                    {
                        bossTransform = Instantiate(bossPrefab, go.transform.position, go.transform.rotation).transform;
                        humanPlayerCount++;
                    }

                    boss = bossTransform.GetComponent<Boss>();
                    boss.SetPlayerConfig(bossPlayerConfig, GameManager.Instance.activeColorSet);
                    bossPlayerConfig.playerTransform = bossTransform;
                }
            }
        }

        switch (humanPlayerCount)
        {
            case 1:
                points.PointLeadToWin = GameManager.Instance.pointsToWinSolo;
                break;
            case 2:
                points.PointLeadToWin = GameManager.Instance.pointsToWinDuo;
                break;
            case 3:
                points.PointLeadToWin = GameManager.Instance.pointsToWinTriple;
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

