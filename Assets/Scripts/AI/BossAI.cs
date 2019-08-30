using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : Boss 
{

    #region Variable Declarations
    [Header("AI Parameters")]
    [SerializeField]
    float repathingDistance = 5f;
    [Range(0, 1)]
    [SerializeField]
    float cornerPeek = 0.2f;

    NavMeshAgent agent;
    Transform redHero;
    Transform blueHero;
    Transform greenHero;
    List<Transform> corners = new List<Transform>();
    List<Transform> middleTargets = new List<Transform>();
    List<Transform> allAITargets = new List<Transform>();
    float randomnessTimer;
    #endregion



    #region Unity Event Functions
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    override protected void Start()
    {
        base.Start();

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
            randomnessTimer += Time.deltaTime;
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



    #region Public Functions
    public void SetHeroReferencesNextFrame()
    {
        StartCoroutine(Wait(1, () =>
        {
            Hero[] heroes = GameObject.FindObjectsOfType<Hero>();
            foreach (Hero hero in heroes)
            {
                if (hero == null) continue;
                if (hero.PlayerColor == PlayerColor.Red) redHero = hero.transform;
                if (hero.PlayerColor == PlayerColor.Blue) blueHero = hero.transform;
                if (hero.PlayerColor == PlayerColor.Green) greenHero = hero.transform;
            }
        }));
    }

    public override void SetMovable(bool active)
    {
        base.SetMovable(active);

        agent.isStopped = !active;
    }
    #endregion



    #region Private Functions
    private void CalculateMovement()
    {
        // Calculate path to strength hero
        NavMeshPath path = new NavMeshPath();
        switch (strengthColor)
        {
            case PlayerColor.Red:
                NavMesh.CalculatePath(transform.position, redHero.position, agent.areaMask, path);

                // strength hero not in shooting range
                if (agent.destination == transform.position && path.corners.Length > 2)
                {
                    // Move
                    Vector3 destination = path.corners[path.corners.Length - 2] + (redHero.position - path.corners[path.corners.Length - 2]) * cornerPeek;
                    SetDestination(destination);
                }

                // Rotate
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(redHero.position - transform.position, Vector3.up), Time.deltaTime * rotateSpeed);
                break;

            case PlayerColor.Blue:
                NavMesh.CalculatePath(transform.position, blueHero.position, agent.areaMask, path);

                // strength hero not in shooting range
                if (agent.destination == transform.position && path.corners.Length > 2)
                {
                    // Move
                    Vector3 destination = path.corners[path.corners.Length - 2] + (blueHero.position - path.corners[path.corners.Length - 2]) * cornerPeek;
                    SetDestination(destination);
                }

                // Rotate
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(blueHero.position - transform.position, Vector3.up), Time.deltaTime * rotateSpeed);
                break;

            case PlayerColor.Green:
                NavMesh.CalculatePath(transform.position, greenHero.position, agent.areaMask, path);

                // strength hero not in shooting range
                if (agent.destination == transform.position && path.corners.Length > 2)
                {
                    // Move
                    Vector3 destination = path.corners[path.corners.Length - 2] + (greenHero.position - path.corners[path.corners.Length - 2]) * cornerPeek;
                    SetDestination(destination);
                }

                // Rotate
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(greenHero.position - transform.position, Vector3.up), Time.deltaTime * rotateSpeed);
                break;
        }
    }

    private void HandleAbilities()
    {
        Attack();
        Ability();
    }

    private void SetDestination(Vector3 destination)
    {
        Vector3 start = transform.position + agent.velocity * (repathingDistance / agent.speed);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(start, destination, agent.areaMask, path);

        agent.SetPath(path);
    }

    private void Attack()
    {
        if (attackCooldownB)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 1.9f + Vector3.up * 0.5f, transform.rotation);
            projectile.GetComponent<BossProjectile>().damage = attackDamagePerShot;
            projectile.GetComponent<BossProjectile>().playerColor = strengthColor;
            projectile.GetComponent<BossProjectile>().lifeTime = attackProjectileLifeTime;
            projectile.GetComponent<Rigidbody>().velocity = transform.forward * attackProjectileSpeed;
            projectile.GetComponent<Renderer>().material.SetColor("_TintColor", activeStrengthColor);

            audioSource.PlayOneShot(attackSound, attackSoundVolume);

            attackCooldownB = false;
            StartCoroutine(ResetAttackCooldown());
        }
    }

    private void Ability()
    {
        if (abilityCooldownB)
        {

            for (int i = 0; i < numberOfProjectiles; ++i)
            {
                float factor = (i / (float)numberOfProjectiles) * Mathf.PI * 2f;
                Vector3 pos = new Vector3(
                    Mathf.Sin(factor) * 1.9f,
                    transform.position.y + 0.5f,
                    Mathf.Cos(factor) * 1.9f);

                GameObject projectile = Instantiate(projectilePrefab, pos + transform.position, Quaternion.identity);
                projectile.GetComponent<BossProjectile>().damage = abilityDamagePerShot;
                projectile.GetComponent<BossProjectile>().playerColor = strengthColor;
                projectile.GetComponent<BossProjectile>().lifeTime = abilityProjectileLifeTime;
                projectile.GetComponent<Rigidbody>().velocity = (projectile.transform.position - transform.position) * abilityProjectileSpeed;
                projectile.GetComponent<Renderer>().material.SetColor("_TintColor", activeStrengthColor);
            }

            audioSource.PlayOneShot(abilitySound, abilitySoundVolume);

            abilityCooldownB = false;
            StartCoroutine(ResetAbilityCooldown());
        }
    }
    #endregion



    #region Coroutines
    IEnumerator Wait (int frames, System.Action onComplete)
    {
        yield return null;

        onComplete.Invoke();
    }
    #endregion
}
