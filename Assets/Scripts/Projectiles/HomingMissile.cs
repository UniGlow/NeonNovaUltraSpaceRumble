using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class HomingMissile : MonoBehaviour
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

    [Header("Camera Shake")]
    [SerializeField] public bool enableCameraShake = true;
    [SerializeField] float magnitude = 5f;
    [SerializeField] float roughness = 10f;
    [SerializeField] float fadeIn = 0.1f;
    [SerializeField] float fadeOut = 0.8f;

    [Header("Object References")]
    [SerializeField] GameObject hitPSHeroes;
    [SerializeField] GameObject hitPSBoss;

    Transform target;
    NavMeshAgent agent;
    AudioSource audioSource;
    bool agentPaused = false;
	#endregion
	
	
	
	#region Unity Event Functions
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains(Constants.TAG_HERO))
        {
            HeroHealth.Instance.TakeDamage(damage);

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSHeroes, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            if (enableCameraShake) EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        }

        else if (other.tag.Contains(Constants.TAG_BOSS))
        {
            BossHealth.Instance.TakeDamage(damage);

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSBoss, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            if (enableCameraShake) EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        }
    }

    private void Update()
    {
        if (!agentPaused)
        {
            if (target == null)
            {
                Debug.LogError("Homing Missile has no target set", this);
                return;
            }

            agent.SetDestination(target.position);
            agent.Move(transform.forward * speed);
        }
    }
    #endregion



    #region Public Functions
    public void AcquireNewTarget(PlayerConfig hero1, PlayerConfig hero2)
    {
        // TODO: String compare ersetzen mit neuer Ability Architektur
        if (hero1.ability.name == "Victim") target = hero1.playerTransform;
        else if (hero2.ability.name == "Victim") target = hero2.playerTransform;
    }

    public void AcquireNewTarget(PlayerConfig hero1, PlayerConfig hero2, PlayerConfig hero3, PlayerConfig boss)
    {
        // TODO: String compare ersetzen mit neuer Ability Architektur
        if (hero1.ability.name == "Victim") target = hero1.playerTransform;
        else if (hero2.ability.name == "Victim") target = hero2.playerTransform;
        else if (hero3.ability.name == "Victim") target = hero3.playerTransform;
    }

    public void PauseMissile(bool pause)
    {
        agent.isStopped = pause;
        agentPaused = pause;
    }
    #endregion
}
