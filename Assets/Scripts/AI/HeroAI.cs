using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class HeroAI : Hero
{

    #region Variable Declarations
    [Header("AI Parameters")]
    [SerializeField] float repathingDistance = 5f;
    [SerializeField] float randomness = 5f;
    [SerializeField] float shieldDelay = 2f;
    [Range(0,1)]
    [SerializeField] float tankFollowSpeed = 0.5f;
    [Range(0,5)]
    [SerializeField] float tankTargetDistance = 1.5f;
    [Range(0,1)]
    [SerializeField] float damageCornerPeek = 0.2f;
    [SerializeField] LayerMask attackRayMask;

    NavMeshAgent agent;
    Transform boss;
    Transform damage;
    List<Transform> corners = new List<Transform>();
    List<Transform> middleTargets = new List<Transform>();
    List<Transform> allAITargets = new List<Transform>();
    int currentlyTargetedCorner;
    float randomnessTimer;
    float normalAgentSpeed;
    #endregion



    #region Unity Event Functions
    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        StartCoroutine(Wait(1, () => 
        {
            Hero[] friends = GameObject.FindObjectsOfType<Hero>();
            foreach (Hero hero in friends)
            {
                if (hero.PlayerConfig.ability.Class == Ability2.AbilityClass.Damage) damage = hero.transform;
            }
        }));

        GameObject[] cornersGO = GameObject.FindGameObjectsWithTag(Constants.TAG_AI_CORNER);
        foreach (GameObject go in cornersGO)
        {
            corners.Add(go.transform);
        }

        GameObject[] middleTargetsGO = GameObject.FindGameObjectsWithTag(Constants.TAG_AI_MIDDLE);
        foreach (GameObject go in middleTargetsGO)
        {
            middleTargets.Add(go.transform);
        }
        
        for (int i = 0; i < corners.Count + middleTargets.Count; i++)
        {
            if (i < corners.Count) allAITargets.Add(corners[i].transform);
            else allAITargets.Add(middleTargets[i - corners.Count].transform);
        }

        normalAgentSpeed = agent.speed;
    }

    new private void FixedUpdate()
    {
        if (active)
        {
            CalculateMovement();
        }
    }

    new private void Update()
    {
        if (active)
        {
            randomnessTimer += Time.deltaTime;

            playerConfig.ability.Tick(Time.deltaTime, CheckTriggerConditions());

            // Apply class-dependant movement speed modifier
            agent.speed = normalAgentSpeed * (playerConfig.ability.SpeedBoost + 1);
            agent.speed = normalAgentSpeed * (playerConfig.ability.SpeedBoost + 1);
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
    public override void SetMovable(bool active)
    {
        base.SetMovable(active);

        agent.isStopped = !active;
    }

    public void SetReferences(PlayerConfig hero1, PlayerConfig hero2, PlayerConfig hero3, PlayerConfig boss)
    {
        this.boss = boss.playerTransform;
    }
    #endregion



    #region Private Functions
    private void CalculateMovement()
    {
        if (playerConfig.ability.Class == Ability2.AbilityClass.Victim)
        {
            if (agent.destination == transform.position) SetDestination(GetRandomTarget());

            if (agent.remainingDistance < repathingDistance)
            {
                if (randomnessTimer >= randomness)
                {
                    SetDestination(GetRandomMiddle());
                    randomnessTimer = 0;
                }
                else SetDestination(GetNextCorner());
            }
        }
        else if (playerConfig.ability.Class == Ability2.AbilityClass.Damage)
        {
            // Move
            SetDamageDestination();

            // Rotate
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.LookRotation(boss.position - transform.position, Vector3.up), 
                Time.deltaTime * characterStats.rotationSpeed);
        }
        else if (playerConfig.ability.Class == Ability2.AbilityClass.Tank)
        {
            SetTankDestination();
        }
    }

    private bool CheckTriggerConditions()
    {
        if (playerConfig.ability.Class == Ability2.AbilityClass.Damage)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 70f, attackRayMask) && hitInfo.transform.tag == Constants.TAG_BOSS)
            {
                Debug.DrawRay(ray.origin, hitInfo.point - ray.origin, Color.green);
                return true;
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 70f, Color.red);
            }
        }
        else if (playerConfig.ability.Class == Ability2.AbilityClass.Tank)
        {
            if (playerConfig.ability.CooldownTimer >= playerConfig.ability.Cooldown + shieldDelay)
            {
                return true;
            }
        }

        return false;
    }

    #region AI Methods
    private void SetDamageDestination()
    {
        // Calculate path to boss
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, boss.position, agent.areaMask, path);
        if (agent.destination == transform.position && path.corners.Length > 2)
        {
            Vector3 destination = path.corners[path.corners.Length - 2] + (boss.position - path.corners[path.corners.Length - 2]) * damageCornerPeek;
            SetDestination(destination);
        }

    }

    private void SetTankDestination()
    {
        Vector3 nearDamage = damage.position + (transform.position - damage.position).normalized * tankTargetDistance;
        if (!SetDestination(nearDamage + (transform.position - nearDamage) * tankFollowSpeed))
        {
            SetDestination(damage.position);
        }
    }

    private bool SetDestination(Vector3 destination)
    {
        Vector3 start = transform.position + agent.velocity * (repathingDistance / agent.speed);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(start, destination, agent.areaMask, path);

        return agent.SetPath(path);
    }

    private Vector3 GetRandomMiddle()
    {
        return middleTargets[Random.Range(0, middleTargets.Count)].position;
    }

    private Vector3 GetRandomTarget() {
        return allAITargets[Random.Range(0, allAITargets.Count)].position;
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
        } else
        {
            currentlyTargetedCorner = 0;
            return corners[currentlyTargetedCorner].position;
        }
    }
    #endregion
    #endregion


    #region Corouintes
    IEnumerator Wait (int frames, System.Action onComplete)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        onComplete.Invoke();
    }
    #endregion
}
