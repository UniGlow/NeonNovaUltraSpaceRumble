using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Boundary : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Constants.TAG_BOSS || other.tag == Constants.TAG_HERO)
        {
            PlayerConfig playerConfig = other.attachedRigidbody.GetComponent<Character>().PlayerConfig;
            GameObject player = other.attachedRigidbody.gameObject;
            
            foreach (GameObject go in GameObject.FindGameObjectsWithTag(Constants.TAG_SPAWN_POINT))
            {
                SpawnPoint spawn = null;
                try
                {
                    spawn = go.GetComponent<SpawnPoint>();
                }
                catch (System.NullReferenceException)
                {
                    Debug.LogError("Search for GameObjects with Tag: " + Constants.TAG_SPAWN_POINT + " has found GameObject: " + go.name + " without SpawnPoint Component!", go);
                }
                if (spawn.objectToSpawn == SpawnPoint.SpawnObject.Boss)
                {
                    if(playerConfig.Faction == Faction.Boss)
                    {
                        player.transform.position = spawn.transform.position;
                        return;
                    }
                }
                else
                {
                    if(playerConfig.Faction == Faction.Heroes)
                    {
                        player.transform.position = spawn.transform.position;
                        return;
                    }
                }
            }
        }
    }
    #endregion



    #region Public Functions

    #endregion



    #region Private Functions

    #endregion



    #region GameEvent Raiser

    #endregion



    #region Coroutines

    #endregion
}

