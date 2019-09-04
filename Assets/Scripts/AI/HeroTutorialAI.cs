using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class HeroTutorialAI : Hero
{

    #region Variable Declarations
    [Header("AI Parameters")]
    [SerializeField] float repathingDistance = 5f;
    [SerializeField] float shieldDelay = 2f;
    [SerializeField] LayerMask attackRayMask;

    NavMeshAgent agent;
    Transform boss;
    List<Transform> corners = new List<Transform>();
    int currentlyTargetedCorner;
    float shieldDelayTimer;
    float normalAgentSpeed;
    #endregion



    #region Unity Event Functions
    override protected void Start()
    {
        base.Start();

        // Get references
        agent = GetComponent<NavMeshAgent>();
        //homingMissile = GameObject.FindGameObjectWithTag(Constants.TAG_HOMING_MISSILE).transform;
        boss = GameObject.FindGameObjectWithTag(Constants.TAG_BOSS).transform.parent;

        GameObject[] cornersGO = GameObject.FindGameObjectsWithTag(Constants.TAG_AI_CORNER);
        foreach (GameObject go in cornersGO)
        {
            corners.Add(go.transform);
        }

        normalAgentSpeed = agent.speed;
    }

    new private void FixedUpdate()
    {
        if (active)
        {
            CalculateMovement();
            HandleAbilities();
        }
    }

    new private void Update()
    {
        if (active)
        {
            if (cooldown) shieldDelayTimer += Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + agent.velocity * (repathingDistance / agent.speed), 1f);


        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            if (i < agent.path.corners.Length - 1) Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i + 1]);
            Gizmos.DrawWireSphere(agent.path.corners[i], 0.3f);
        }
    }
    #endregion



    #region Public Funtcions

    #endregion



    #region Private Functions
    private void CalculateMovement()
    {
        if (ability == Ability.Opfer)
        {
            if (agent.destination == transform.position) SetDestination(GetNextCorner());

            if (agent.remainingDistance < repathingDistance)
            {
                SetDestination(GetNextCorner());
            }
        }

        else if (ability == Ability.Damage)
        {
            // Rotate
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(boss.position - transform.position, Vector3.up), Time.deltaTime * rotateSpeed);
        }
    }

    private void HandleAbilities()
    {
        if (ability == Ability.Opfer)
        {
            Run();
        }
        else if (ability == Ability.Damage && cooldown)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
            RaycastHit hitInfo;

            Debug.DrawRay(ray.origin, ray.direction * 70f, Color.red);
            if (Physics.Raycast(ray, out hitInfo, 70f, attackRayMask)/* && hitInfo.transform == boss*/)
            {
                Debug.DrawRay(ray.origin, hitInfo.point, Color.green);
                Attack();
            }
        }
        else if (ability == Ability.Tank)
        {
            if (shieldDelayTimer >= shieldDelay)
            {
                Defend();
                shieldDelayTimer = 0;
            }
        }
    }

    #region AI Methods
    private bool SetDestination(Vector3 destination)
    {
        Vector3 start = transform.position + agent.velocity * (repathingDistance / agent.speed);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(start, destination, agent.areaMask, path);

        return agent.SetPath(path);
    }

    /// <summary>
    /// Returns the position of the closest corner of the level and (0,0,0) if no corner is found.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetClosestCorner()
    {
        float closestDistance = 100f;
        Vector3 closestCorner = Vector3.zero;
        for (int i = 0; i < corners.Count; i++)
        {
            if (Vector3.Distance(corners[i].position, transform.position) < closestDistance)
            {
                closestCorner = corners[i].position;
                currentlyTargetedCorner = i;
            }
        }
        return closestCorner;
    }

    private Vector3 GetNextCorner()
    {
        if (currentlyTargetedCorner < corners.Count - 1)
        {
            currentlyTargetedCorner++;
            return corners[currentlyTargetedCorner].position;
        }
        else
        {
            currentlyTargetedCorner = 0;
            return corners[currentlyTargetedCorner].position;
        }
    }
    #endregion

    private void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.GetComponent<HeroProjectile>().damage = damagePerShot;
        projectile.GetComponent<HeroProjectile>().playerColor = PlayerConfig.ColorConfig;
        projectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;

        audioSource.PlayOneShot(attackSound, attackSoundVolume);

        cooldown = false;
        StartCoroutine(ResetAttackCooldown());
    }

    private void Defend()
    {
        wobbleBobble.SetActive(true);
        cooldown = false;
        cooldownIndicator.sprite = defendCooldownSprites[0];
        audioSource.PlayOneShot(wobbleBobbleSound, wobbleBobbleVolume);
        resetDefendCoroutine = StartCoroutine(ResetDefend());
    }

    private void Run()
    {
        agent.speed = normalAgentSpeed * (speedBoost + 1);
        agent.speed = normalAgentSpeed * (speedBoost + 1);
    }
    #endregion
}
