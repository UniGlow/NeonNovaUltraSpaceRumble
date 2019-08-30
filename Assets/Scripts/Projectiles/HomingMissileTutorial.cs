using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class HomingMissileTutorial : SubscribedBehaviour
{

    #region Variable Declarations
    [SerializeField] float speed = 10f;
    [SerializeField] int damage = 100;

    [Header("Sound")]
    [SerializeField]
    AudioClip hitSound;
    [Range(0, 1)]
    [SerializeField]
    float hitSoundVolume = 1f;

    [Header("Object References")]
    [SerializeField] GameObject hitPSHeroes;
    [SerializeField] GameObject hitPSBoss;

    Transform target;
    NavMeshAgent agent;
    AudioSource audioSource;
    bool agentPaused = false;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        AcquireNewTarget();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains(Constants.TAG_HERO_DUMMY))
        {
            HeroHealth.Instance.TakeDamage(damage);

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSHeroes, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }

        else if (other.tag.Contains(Constants.TAG_BOSS_DUMMY))
        {
            BossHealth.Instance.TakeDamage(damage);

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSBoss, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }

    private void Update()
    {
        if (!agentPaused)
        {
            if (target == null) AcquireNewTarget();

            agent.SetDestination(target.position);
            agent.Move(transform.forward * speed);
        }
    }
    #endregion



    #region Public Functions
    public void AcquireNewTarget()
    {
        Hero[] heroes = GameObject.FindObjectsOfType<HeroTutorialAI>();
        foreach (Hero hero in heroes) {
            if (hero.ability == Ability.Opfer) {
                target = hero.transform;
            }
        }
    }

    public void PauseMissile(bool pause)
    {
        agentPaused = pause;
    }
    #endregion
}
