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
    [SerializeField] float rotateSpeed = 200f;
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
    Rigidbody rb;
    NavMeshAgent agent;
    AudioSource audioSource;
    bool agentPaused = false;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        AcquireNewTarget();
	}

    private void OnTriggerEnter(Collider other) {
        if (other.tag.Contains(Constants.TAG_HERO)) {
            HeroHealth.Instance.TakeDamage(damage);
            other.transform.parent.GetComponent<Transmission>().EndTransmission();

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSHeroes, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
        else if (other.tag.Contains(Constants.TAG_BOSS)) {
            BossHealth.Instance.TakeDamage(damage);

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSBoss, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
        else if (other.tag.Contains(Constants.TAG_WALL)) {

        }
    }

    private void Update() {
        if (!agentPaused) {
            agent.SetDestination(target.position);
            agent.Move(transform.forward * speed);
        }
    }
    #endregion



    #region Custom Event Functions
    protected override void OnLevelCompleted() {
        PauseMissile(true);
    }
    #endregion



    #region Public Functions
    public void AcquireNewTarget() {
        Hero[] heroes = GameObject.FindObjectsOfType<Hero>();
        foreach (Hero hero in heroes) {
            if (hero.ability == Ability.Opfer) {
                target = hero.transform;
            }
        }
    }

    public void PauseMissile(bool pause) {
        agentPaused = pause;
    }
    #endregion



    private void ManualMovement() {
        Vector3 direction = (target.position - rb.position).normalized;

        Vector3 rotateAmount = Vector3.Cross(transform.forward, direction);

        rb.angularVelocity = rotateAmount * rotateSpeed;

        rb.velocity = transform.forward * speed;
    }
}
