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
    [System.Serializable]
    public class HitDetails
    {
        public Character character = null;
        public float timeStamp = 0f;
    }




    #region Variable Declarations
    [Header("Stats")]
    [SerializeField] float speed = 10f;
    [SerializeField] int damage = 100;
    [Tooltip("Determines the duration in seconds after a hit in which the target will be invulnerable against the orb.")]
    [SerializeField] float invulnerabilityAfterHit = 1f;

    [Header("Sound")]
    [SerializeField]
    AudioClip hitSound = null;
    [Range(0, 1)]
    [SerializeField]
    float hitSoundVolume = 1f;

    [Header("Camera Shake")]
    [SerializeField] public bool enableCameraShake = true;
    [SerializeField] float magnitude = 5f;
    [SerializeField] float roughness = 10f;
    [SerializeField] float fadeIn = 0.1f;
    [SerializeField] float fadeOut = 0.8f;

    [Header("Rumble")]
    [Range(0f,1f)]
    [SerializeField] float rumbleStrengthDeep = 1f;
    [Range(0f, 1f)]
    [SerializeField] float rumbleStrengthHigh = 1f;
    [SerializeField] float rumbleDuration = 0.5f;

    [Header("Particle Systems")]
    [SerializeField] GameObject hitPSHeroes = null;
    [SerializeField] GameObject hitPSBoss = null;

    [Header("References")]
    [SerializeField] Points points = null;
    [Tooltip("Can be used to set a fixed target in the scene.")]
    [SerializeField] Transform fixedTarget = null;

    Transform target;
    NavMeshAgent agent;
    AudioSource audioSource;
    bool agentPaused = false;
    bool gameStarted = false;
    List<HitDetails> recentHits = new List<HitDetails>();
    #endregion



    #region Public Properties
    public float Speed { get { return speed; } set { speed = value; } }
    #endregion



    #region Unity Event Functions
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (fixedTarget) target = fixedTarget;
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

        // Don't do anything if you just hit this character
        if (recentHits.Find(x => x.character == character) != null) return;

        if (other.tag.Contains(Constants.TAG_HERO))
        {
            points.ScorePoints(Faction.Boss, damage);
            if (!fixedTarget) target.GetComponent<Hero>().PlayerConfig.HeroScore.CurrentLevelScore.RunnerScore.RegisterOrbHit(Faction.Heroes);

            recentHits.Add(new HitDetails
            {
                character = character,
                timeStamp = Time.timeSinceLevelLoad
            });

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSHeroes, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            if (enableCameraShake) EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);

            if (!fixedTarget && !character.PlayerConfig.AIControlled)
            {
                character.PlayerConfig.Player.SetVibration(0, rumbleStrengthDeep, rumbleDuration, false);
                character.PlayerConfig.Player.SetVibration(1, rumbleStrengthHigh, rumbleDuration, false);
            }
        }

        else if (other.tag.Contains(Constants.TAG_BOSS))
        {
            points.ScorePoints(Faction.Heroes, damage);
            target.GetComponent<Hero>().PlayerConfig.HeroScore.CurrentLevelScore.RunnerScore.RegisterOrbHit(Faction.Boss);

            recentHits.Add(new HitDetails
            {
                character = character,
                timeStamp = Time.timeSinceLevelLoad
            });

            audioSource.PlayOneShot(hitSound, hitSoundVolume);

            Instantiate(hitPSBoss, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            if (enableCameraShake) EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);

            if (!character.PlayerConfig.AIControlled)
            {
                character.PlayerConfig.Player.SetVibration(0, rumbleStrengthDeep, rumbleDuration, false);
                character.PlayerConfig.Player.SetVibration(1, rumbleStrengthHigh, rumbleDuration, false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (gameStarted)
        {
            if (agentPaused) return;

            if (target == null)
            {
                Debug.LogError("Homing Missile has no target set", this);
                return;
            }

            agent.SetDestination(target.position);
            agent.Move(transform.forward * speed);
            UpdateRecentHits();
        }
        else
        {
            return;
        }
    }
    #endregion



    #region Public Functions
    public void AcquireNewTarget(PlayerConfig hero1, PlayerConfig hero2)
    {
        if (hero1.Ability.Class == Ability.AbilityClass.Runner) target = hero1.playerTransform;
        else if (hero2.Ability.Class == Ability.AbilityClass.Runner) target = hero2.playerTransform;
    }

    public void AcquireNewTarget(PlayerConfig hero1, PlayerConfig hero2, PlayerConfig hero3, PlayerConfig boss)
    {
        if (hero1.Ability.Class == Ability.AbilityClass.Runner) target = hero1.playerTransform;
        else if (hero2.Ability.Class == Ability.AbilityClass.Runner) target = hero2.playerTransform;
        else if (hero3.Ability.Class == Ability.AbilityClass.Runner) target = hero3.playerTransform;
    }

    public void PauseMissile(bool pause)
    {
        agent.isStopped = pause;
        agentPaused = pause;
    }

    public void StartGame(bool started)
    {
        agent.isStopped = !started;
        gameStarted = started;
    }
    #endregion



    #region Private Functions
    void UpdateRecentHits()
    {
        //foreach (HitDetails hit in recentHits)
        //{
        //    if (Time.timeSinceLevelLoad >= hit.timeStamp + invulnerabilityAfterHit) recentHits.Remove(hit);
        //}

        for (int i = recentHits.Count-1; i >= 0; i--)
        {
            if (Time.timeSinceLevelLoad >= recentHits[i].timeStamp + invulnerabilityAfterHit) recentHits.Remove(recentHits[i]);
        }
    }
    #endregion
}
