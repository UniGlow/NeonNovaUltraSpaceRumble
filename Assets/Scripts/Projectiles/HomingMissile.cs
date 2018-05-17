using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class HomingMissile : SubscribedBehaviour {

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
	private void Start() {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        AcquireNewTarget();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains(Constants.TAG_HERO)) {
            HeroHealth.Instance.TakeDamage(damage);
            other.transform.parent.GetComponent<Transmission>().EndTransmission();

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSHeroes, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        }

        else if (other.tag.Contains(Constants.TAG_BOSS)) {
            BossHealth.Instance.TakeDamage(damage);

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSBoss, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        }
    }

    private void Update()
    {
        if (!agentPaused) {
            if (target == null) AcquireNewTarget();

            agent.SetDestination(target.position);
            agent.Move(transform.forward * speed);
        }
    }
    #endregion



    #region Custom Event Functions
    override protected void OnLevelCompleted(string winner)
    {
        PauseMissile(true);
    }

    protected override void OnLevelStarted()
    {
        PauseMissile(false);
    }
    #endregion



    #region Public Functions
    public void AcquireNewTarget()
    {
        Hero[] heroes = GameObject.FindObjectsOfType<Hero>();
        foreach (Hero hero in heroes) {
            if (hero.ability == Ability.Opfer && hero.GetType() != typeof(HeroTutorialAI)) {
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
