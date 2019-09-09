using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class HomingMissileTutorial : MonoBehaviour
{

    #region Variable Declarations
    [SerializeField] float speed = 10f;

    [Header("Sound")]
    [SerializeField]
    AudioClip hitSound;
    [Range(0, 1)]
    [SerializeField]
    float hitSoundVolume = 1f;

    [Header("Object References")]
    [SerializeField] GameObject hitPSHeroes;
    [SerializeField] GameObject hitPSBoss;
    [SerializeField] Transform target;

    NavMeshAgent agent;
    AudioSource audioSource;
    bool agentPaused = false;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains(Constants.TAG_HERO_DUMMY))
        {
            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSHeroes, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }

        else if (other.tag.Contains(Constants.TAG_BOSS_DUMMY))
        {
            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSBoss, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }

    private void Update()
    {
        if (!agentPaused)
        {
            agent.SetDestination(target.position);
            agent.Move(transform.forward * speed);
        }
    }
    #endregion



    #region Public Functions
    public void PauseMissile(bool pause)
    {
        agentPaused = pause;
    }
    #endregion
}
