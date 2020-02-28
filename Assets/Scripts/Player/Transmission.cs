using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using DG.Tweening;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Hero))]
[RequireComponent(typeof(AudioSource))]
public class Transmission : MonoBehaviour
{
    [System.Serializable]
    public class Receiver
    {
        public Transmission transmitter = null;
        public Color color = new Color();

        public Receiver(Transmission transmitter, Color color)
        {
            this.transmitter = transmitter;
            this.color = color;
        }
    }

    public enum State
    {
        Deactivated,
        Searching,
        ReadyToTransmit,
        Transmitting
    }

    #region Variable Declarations
    [Header("Settings")]
    [SerializeField] protected float transmissionRange = 5f;
    [SerializeField] protected float transmissionDuration = 3f;
    [SerializeField] protected float transmissionCooldown = 1f;

    [Header("Sound")]
    [SerializeField]
    protected AudioClip transmissionSound;
    [Range(0, 1)]
    [SerializeField]
    protected float transmissionSoundVolume = 1f;

    [Header("Rumble")]
    [Tooltip("Rumble strength when a hero is targeted for a switch by another hero.")]
    [Range(0f, 1f)]
    [SerializeField] protected float rumbleStrengthInRangeDeep = 0.2f;
    [Tooltip("Rumble strength when a hero is targeted for a switch by another hero.")]
    [Range(0f, 1f)]
    [SerializeField] protected float rumbleStrengthInRangeHigh = 0.2f;

    [Space]
    [Range(0f, 1f)]
    [SerializeField] protected float rumbleStrengthOnSwitchDeep = 0.4f;
    [Range(0f, 1f)]
    [SerializeField] protected float rumbleStrengthOnSwitchHigh = 0.4f;
    [SerializeField] protected float rumbleDurationOnSwitch = 0.4f;

    [Header("Mesh Blink")]
    [Range(0f,10f)]
    [SerializeField] protected float inRangeColorMultiplier = 1f;
    [Range(0f,10f)]
    [SerializeField] protected float switchColorMultiplier = 1f;

    [Space]
    [Range(0f, 1f)]
    [Tooltip("Duration of the blinking of the mesh. Percentual value from transmissionDuration.")]
    [SerializeField] protected float blinkDuration = 0.23f;

    [Header("Shooting Stars")]
    [Range(0f, 1f)]
    [Tooltip("Duration of the travel for the shooting stars. Percentual value from transmissionDuration.")]
    [SerializeField] protected float shootingStarsDuration = 0.77f;
    [SerializeField] protected Ease shootingStarEase = Ease.OutSine;
    [Range(-1f, 1f)]
    [SerializeField] protected float shootingStarsOffset = 0.08f;

    [Header("Transparent Mesh")]
    [Range(0f, 1f)]
    [Tooltip("Duration of the alpha fade for the transparent mesh. Percentual value from transmissionDuration.")]
    [SerializeField] protected float meshFadeDuration = 0.77f;
    [SerializeField] protected Ease meshFadeEase = Ease.InQuad;

    [Space]
    [Range(0f, 1f)]
    [Tooltip("Duration of the scaling for the transparent mesh. Percentual value from transmissionDuration.")]
    [SerializeField] protected float meshScaleDuration = 0.77f;
    [SerializeField] protected Ease meshScaleEase = Ease.OutCubic;

    [Header("Slow Motion")]
    [Range(0f, 1f)]
    [SerializeField] protected float freezeFrameDuration = 0.1f;
    [Range(0f, 1f)]
    [Tooltip("Percentual value from realtime (realtime = 1).")]
    [SerializeField] protected float slowMotionStrength = 0.2f;
    [Range(0f, 1f)]
    [Tooltip("Percentual value from realtime (realtime = 1).")]
    [SerializeField] protected float musicPitchStrength = 0.2f;
    [Range(0f, 1f)]
    [Tooltip("Duration of the fade in for the slow motion. Percentual value from transmissionDuration.")]
    [SerializeField] protected float slowMotionFadeInDuration = 0.25f;
    [Range(0f, 1f)]
    [Tooltip("Duration of the fade out for the slow motion. Percentual value from transmissionDuration.")]
    [SerializeField] protected float slowMotionFadeOutDuration = 0.5f;

    [Header("References")]
    [SerializeField] protected GameObject transmissionRangeIndicator;
    [SerializeField] protected ParticleSystem transmissionPS;
    [SerializeField] protected GameEvent abilitySwitchInitiatedEvent = null;
    [SerializeField] protected GameEvent abilitiesChangedEvent = null;
    [SerializeField] protected GameObject shootingStarsPrefab = null;
    [SerializeField] protected GameObject transparentPlayerMeshPrefab = null;

    protected Hero hero;
    protected AudioSource audioSource;
    protected Material playerMat;
    protected State state;
    protected List<Receiver> receivers = new List<Receiver>();
    protected Transmission transmissionPartner;
    protected Ability receivingAbility;
    protected bool isTargeted;
    protected bool transmissionReady = true;
    #endregion



    #region Public Properties
    public ParticleSystem TransmissionPS { get { return transmissionPS; } }
    #endregion



    #region Unity Event Functions
    virtual protected void Start()
    {
        hero = GetComponent<Hero>();
        audioSource = GetComponent<AudioSource>();


        // Hero PlayerConfig is null in Credits Scene
        if(hero.PlayerConfig != null)
        {
            playerMat = hero.PlayerMesh.GetComponent<MeshRenderer>().material;
            transmissionRangeIndicator.transform.localScale = new Vector3(transmissionRange * 0.2f, transmissionRange * 0.2f, transmissionRange * 0.2f);
            ParticleSystem[] rangeParticleSystems = transmissionRangeIndicator.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in rangeParticleSystems)
            {
                var main = ps.main;
                main.startColor = 
                    new Color(hero.PlayerConfig.ColorConfig.staubsaugerColor.r, 
                    hero.PlayerConfig.ColorConfig.staubsaugerColor.g, 
                    hero.PlayerConfig.ColorConfig.staubsaugerColor.b, 
                    ps.main.startColor.color.a);
            }
        }
    }
	
	virtual protected void Update()
    {
        switch (state)
        {
            case State.Deactivated:
                if (TransmissionButtonsPressed() && transmissionReady)
                {
                    ChangeState(State.Searching);
                }
                break;

            case State.Searching:
                if (TransmissionButtonsUp())
                {
                    ChangeState(State.Deactivated);
                }

                else if (FindReceiversCircle() > 0)
                {
                    ChangeState(State.ReadyToTransmit);
                }
                break;

            case State.ReadyToTransmit:
                if (TransmissionButtonsUp())
                {
                    ChangeState(State.Deactivated);
                }

                else if (FindReceiversCircle() == 0)
                {
                    ChangeState(State.Searching);
                }

                foreach (Receiver receiver in receivers)
                {
                    // Two heroes ready? => Start transmitting
                    if (receiver.transmitter.state == State.Searching || receiver.transmitter.state == State.ReadyToTransmit)
                    {
                        receiver.transmitter.transmissionPartner = this;
                        receiver.transmitter.receivingAbility = hero.PlayerConfig.Ability;
                        receiver.transmitter.ChangeState(State.Transmitting);
                        transmissionPartner = receiver.transmitter;
                        receivingAbility = receiver.transmitter.hero.PlayerConfig.Ability;
                        ChangeState(State.Transmitting);
                        break;
                    }
                }

                break;

            case State.Transmitting:
                break;

            default:
                break;
        }
    }
    #endregion



    #region Private Functions
    /// <summary>
    /// Shoots a ray forward to look for a receiver (old version of transmission)
    /// </summary>
    /// <returns></returns>
    bool FindReceiverRay()
    {
        RaycastHit hitInfo;
        // Send 3 rays for a casual aiming
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hitInfo, transmissionRange, 1 << 8)
            || Physics.Raycast(transform.position + Vector3.up * 0.5f + Vector3.right * 0.3f, transform.forward, out hitInfo, transmissionRange, 1 << 8)
            || Physics.Raycast(transform.position + Vector3.up * 0.5f - Vector3.right * 0.3f, transform.forward, out hitInfo, transmissionRange, 1 << 8))
        {
            Debug.DrawLine(transform.position + Vector3.up * 0.5f, hitInfo.point, Color.green);
            RemoveAllReceivers();
            AddReceiver(new Receiver(hitInfo.transform.GetComponentInParent<Transmission>(), hitInfo.transform.GetComponentInParent<Hero>().PlayerConfig.ColorConfig.heroMaterial.GetColor("_Color")));
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * transmissionRange, Color.red);
            return false;
        }
    }

    /// <summary>
    /// Searches receiving heroes in a sphere.
    /// </summary>
    /// <returns>Returns the number of receivers found.</returns>
    int FindReceiversCircle()
    {
        // Do the OverlapSphere and setup references
        List<Collider> hits = new List<Collider>(Physics.OverlapSphere(transform.position, transmissionRange, 1 << 8));
        List<Receiver> heroesHit = new List<Receiver>();
        foreach (Collider hit in hits)
        {
            Transmission hitTransmitter = hit.transform.GetComponentInParent<Transmission>();
            if (hitTransmitter != this && hitTransmitter != null) heroesHit.Add(new Receiver(hitTransmitter, hitTransmitter.hero.PlayerConfig.ColorConfig.heroMaterial.GetColor("_Color")));
        }

        // Receivers found
        if (heroesHit.Count > 0)
        {
            // Inform receivers that aren't any longer targeted and remove them from the list
            for (int i = receivers.Count-1; i >= 0; i--)
            {
                if (!heroesHit.Exists(x => x.transmitter == receivers[i].transmitter))
                {
                    RemoveReceiver(receivers[i]);
                }
            }

            // Inform new receivers and add them to the list
            foreach (Receiver hit in heroesHit)
            {
                Debug.DrawLine(transform.position + Vector3.up * 0.5f, hit.transmitter.transform.position, Color.green);

                if (!receivers.Exists(x => x.transmitter == hit.transmitter))
                {
                    AddReceiver(hit);
                }
            }
        }

        // No receiver found
        else
        {
            // Debug outputs
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * transmissionRange, Color.red);
            for (int i = 0; i < 360; i += 20)
            {
                Vector3 direction = Quaternion.Euler(0, i, 0) * transform.forward;
                Debug.DrawRay(transform.position + Vector3.up * 0.5f, direction * transmissionRange, Color.red);
            }

            RemoveAllReceivers();
        }

        return receivers.Count;
    }

    protected void Transmit()
    {
        // Things to do only once (both players are performing Transmit()
        if (transmissionPartner.transmissionPartner == null)
        {
            RaiseAbilitySwitchInitiated(hero.PlayerConfig, transmissionPartner.hero.PlayerConfig, transmissionDuration);

            // FreezeFrame and Slow Motion
            JuiceLib.TimeFX.FreezeFrame(transmissionDuration * freezeFrameDuration, () =>
            {
                JuiceLib.TimeFX.BendTime(slowMotionStrength, transmissionDuration * slowMotionFadeInDuration);
            });
            AudioManager.Instance.BendTime(musicPitchStrength, transmissionDuration * slowMotionFadeInDuration);
        }

        hero.PlayerConfig.Player.SetVibration(0, rumbleStrengthOnSwitchDeep, rumbleDurationOnSwitch);
        hero.PlayerConfig.Player.SetVibration(1, rumbleStrengthOnSwitchHigh, rumbleDurationOnSwitch);

        // Blinking Hero
        playerMat.DOBlendableColor(playerMat.color * switchColorMultiplier, (blinkDuration * transmissionDuration) / 2).SetUpdate(true).OnComplete(() => 
        {
            playerMat.DOBlendableColor(hero.PlayerConfig.ColorConfig.heroMaterial.color, (blinkDuration * transmissionDuration) / 2).SetUpdate(true);
            // Shooting Stars go!
            Vector3 directionToReceiver = transmissionPartner.transform.position - transform.position;
            GameObject shootingStar = Instantiate(shootingStarsPrefab, transform.position + Vector3.Cross(directionToReceiver, Vector3.down) * shootingStarsOffset, Quaternion.LookRotation(directionToReceiver));
            Tweener tween = null;
            tween = shootingStar.transform.DOMove(transmissionPartner.transform.position + Vector3.Cross(directionToReceiver, Vector3.down) * shootingStarsOffset, 
                shootingStarsDuration * transmissionDuration).SetUpdate(true).SetEase(Ease.OutSine);
            // Keep track of target and correct path
            tween.OnUpdate(() => 
            {
                if (tween.Duration() > 0.05f) tween.ChangeEndValue(transmissionPartner.transform.position + Vector3.Cross(directionToReceiver, Vector3.down) * shootingStarsOffset, tween.Duration() - tween.Elapsed(), true);
            }
            ).OnComplete(() => 
            {
                Destroy(shootingStar);
                // Blink again
                playerMat.DOBlendableColor(playerMat.color * switchColorMultiplier, (blinkDuration * transmissionDuration) / 2).SetUpdate(true).SetLoops(2, LoopType.Yoyo);

                // Spawn transparent mesh of new hero shape
                GameObject transparentMesh = Instantiate(transparentPlayerMeshPrefab, transform.position, transform.rotation);
                transparentMesh.GetComponent<MeshFilter>().mesh = receivingAbility.Mesh;
                
                // Set color
                Color transparentMeshColor = hero.PlayerConfig.ColorConfig.heroMaterial.GetColor("_EmissionColor");
                transparentMeshColor.a = 0.2f;
                transparentMesh.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", transparentMeshColor);
                
                // Fade alpha
                transparentMesh.GetComponent<MeshRenderer>().material.DOFade(1f, meshFadeDuration * transmissionDuration).SetEase(Ease.InQuad);

                // Shrink into hero
                transparentMesh.transform.DOScale(1f, meshScaleDuration * transmissionDuration).SetUpdate(true).SetEase(Ease.OutCubic).OnUpdate(() => 
                {
                    transparentMesh.transform.position = transform.position;
                }).OnComplete(() => 
                {
                    Destroy(transparentMesh);
                });

                // Switch abilities
                hero.SetAbility(receivingAbility);

                // More FX
                transmissionPS.Play();

                // Things to do only once (both players are performing Transmit()
                if (transmissionPartner.transmissionPartner == null)
                {
                    RaiseAbilitiesChanged(hero.PlayerConfig, transmissionPartner.hero.PlayerConfig);

                    audioSource.PlayOneShot(transmissionSound, transmissionSoundVolume);

                    // Bend time back to normal
                    if (Time.timeScale != 0) JuiceLib.TimeFX.BendTime(1f, transmissionDuration * slowMotionFadeOutDuration);
                    if (Time.timeScale != 0) AudioManager.Instance.BendTime(1f, transmissionDuration * slowMotionFadeOutDuration);
                }

                EndTransmission();
            });
        });
    }

    protected bool TransmissionButtonsUp()
    {
        if (hero.PlayerConfig.Player.GetButtonUp(RewiredConsts.Action.TRANSMIT_ABILITY))
        {
            return true;
        }

        return false;
    }

    protected bool TransmissionButtonsPressed()
    {
        if (hero.PlayerConfig.Player.GetButtonDown(RewiredConsts.Action.TRANSMIT_ABILITY)) return true;

        return false;
    }

    protected void ChangeState(State newState)
    {
        if (newState == State.Searching || newState == State.ReadyToTransmit)
        {
            transmissionRangeIndicator.SetActive(true);
        }
        else
        {
            transmissionRangeIndicator.SetActive(false);
        }

        if (newState == State.Deactivated)
        {
            RemoveAllReceivers();
        }

        if (newState == State.Transmitting)
        {
            transmissionReady = false;
            Transmit();
        }

        if (newState == State.ReadyToTransmit)
        {
            playerMat.DOBlendableColor(hero.PlayerConfig.ColorConfig.heroMaterial.color * inRangeColorMultiplier, (blinkDuration * transmissionDuration) / 2);
        }

        if (state == State.ReadyToTransmit && newState != State.Transmitting)
        {
            playerMat.DOBlendableColor(hero.PlayerConfig.ColorConfig.heroMaterial.color, (blinkDuration * transmissionDuration) / 2);
        }

        state = newState;
    }

    protected void EndTransmission()
    {
        transmissionPartner = null;
        receivingAbility = null;
        StartCoroutine(TransmissionCooldown());
        ChangeState(State.Deactivated);
    }

    protected void AddReceiver(Receiver receiver)
    {
        receiver.transmitter.isTargeted = true;
        receiver.transmitter.playerMat.DOBlendableColor(receiver.color * inRangeColorMultiplier, 0.3f);
        receiver.transmitter.hero.PlayerConfig.Player.SetVibration(0, rumbleStrengthInRangeDeep);
        receiver.transmitter.hero.PlayerConfig.Player.SetVibration(1, rumbleStrengthInRangeHigh);
        receivers.Add(receiver);
    }

    protected void RemoveReceiver(Receiver receiver)
    {
        receiver.transmitter.isTargeted = false;
        receiver.transmitter.playerMat.DOBlendableColor(receiver.color, 0.3f);
        if (rumbleStrengthInRangeDeep > 0f) receiver.transmitter.hero.PlayerConfig.Player.SetVibration(0, 0f);
        if (rumbleStrengthInRangeHigh > 0f) receiver.transmitter.hero.PlayerConfig.Player.SetVibration(1, 0f);
        receivers.Remove(receiver);
    }

    protected void RemoveAllReceivers()
    {
        for (int i = receivers.Count-1; i >= 0; i--)
        {
            RemoveReceiver(receivers[i]);
        }
    }
    #endregion



    #region GameEvent Raiser
    void RaiseAbilitiesChanged(PlayerConfig hero1Config, PlayerConfig hero2Config)
    {
        abilitiesChangedEvent.Raise(this, hero1Config, hero2Config);
    }

    void RaiseAbilitySwitchInitiated(PlayerConfig hero1Config, PlayerConfig hero2Config, float duration)
    {
        abilitySwitchInitiatedEvent.Raise(this, hero1Config, hero2Config, duration);
    }
    #endregion



    #region Coroutines
    IEnumerator TransmissionCooldown()
    {
        yield return new WaitForSeconds(transmissionCooldown);
        transmissionReady = true;
    }
    #endregion
}
