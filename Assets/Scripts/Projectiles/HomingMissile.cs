using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class HomingMissile : SubscribedBehaviour {

    #region Variable Declarations
    [SerializeField] float speed = 10f;
    [SerializeField] float rotateSpeed = 200f;
    [SerializeField] int damage = 100;

    Transform target;
    Rigidbody rb;
    NavMeshAgent agent;
    bool agentPaused = false;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start() {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        AcquireNewTarget();
	}

    private void OnTriggerEnter(Collider other) {
        if (other.tag.Contains(Constants.TAG_HERO)) {
            HeroHealth.Instance.TakeDamage(damage);
            other.transform.parent.GetComponent<Transmission>().EndTransmission();
        }
        else if (other.tag.Contains(Constants.TAG_BOSS)) {
            BossHealth.Instance.TakeDamage(damage);
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
