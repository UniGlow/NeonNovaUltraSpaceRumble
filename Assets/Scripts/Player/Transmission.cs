using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("FX")]
    [SerializeField]
    protected AudioClip transmissionSound;
    [Range(0, 1)]
    [SerializeField]
    protected float transmissionSoundVolume = 1f;

    [Space]
    [Range(0f,10f)]
    [SerializeField] protected float inRangeColorMultiplier = 1f;
    [Range(0f,10f)]
    [SerializeField] protected float switchColorMultiplier = 1f;

    [Space]
    [Range(0f, 1f)]
    [Tooltip("Duration of the blinking of the mesh. Percentual value from transmissionDuration.")]
    [SerializeField] protected float blinkDuration = 0.23f;

    [Space]
    [Range(0f, 1f)]
    [Tooltip("Duration of the travel for the shooting stars. Percentual value from transmissionDuration.")]
    [SerializeField] protected float shootingStarsDuration = 0.77f;
    [SerializeField] protected Ease shootingStarEase = Ease.OutSine;
    [Range(-1f, 1f)]
    [SerializeField] protected float shootingStarsOffset = 0.08f;

    [Space]
    [Range(0f, 1f)]
    [Tooltip("Duration of the alpha fade for the transparent mesh. Percentual value from transmissionDuration.")]
    [SerializeField] protected float meshFadeDuration = 0.77f;
    [SerializeField] protected Ease meshFadeEase = Ease.InQuad;

    [Space]
    [Range(0f, 1f)]
    [Tooltip("Duration of the scaling for the transparent mesh. Percentual value from transmissionDuration.")]
    [SerializeField] protected float meshScaleDuration = 0.77f;
    [SerializeField] protected Ease meshScaleEase = Ease.OutCubic;

    [Header("References")]
    [SerializeField] protected GameObject transmissionRangeIndicator;
    [SerializeField] protected ParticleSystem transmissionPS;
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
    #endregion



    #region Public Properties
    public ParticleSystem TransmissionPS { get { return transmissionPS; } }
    #endregion



    #region Unity Event Functions
    virtual protected void Start()
    {
        hero = GetComponent<Hero>();
        audioSource = GetComponent<AudioSource>();

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
	
	virtual protected void Update()
    {
        switch (state)
        {
            case State.Deactivated:
                if (TransmissionButtonsPressed())
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
                        receiver.transmitter.receivingAbility = hero.PlayerConfig.ability;
                        receiver.transmitter.ChangeState(State.Transmitting);
                        transmissionPartner = receiver.transmitter;
                        receivingAbility = receiver.transmitter.hero.PlayerConfig.ability;
                        ChangeState(State.Transmitting);
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
        hero.SetMovable(false);

        // Blinking Hero
        playerMat.DOBlendableColor(playerMat.color * switchColorMultiplier, (blinkDuration * transmissionDuration) / 2).OnComplete(() => 
        {
            playerMat.DOBlendableColor(hero.PlayerConfig.ColorConfig.heroMaterial.color, (blinkDuration * transmissionDuration) / 2);
            // Shooting Stars go!
            // TODO: Shooting Stars need to track their targets
            // TODO: Shooting Stars slightly offset
            Vector3 directionToReceiver = transmissionPartner.transform.position - transform.position;
            GameObject shootingStar = Instantiate(shootingStarsPrefab, transform.position + Vector3.Cross(directionToReceiver, Vector3.down) * shootingStarsOffset, Quaternion.LookRotation(directionToReceiver));
            shootingStar.transform.DOMove(transmissionPartner.transform.position + Vector3.Cross(directionToReceiver, Vector3.down) * shootingStarsOffset, shootingStarsDuration * transmissionDuration).SetEase(Ease.OutSine).OnComplete(() => 
            {
                Destroy(shootingStar);
                // Blink again
                playerMat.DOBlendableColor(playerMat.color * switchColorMultiplier, (blinkDuration * transmissionDuration) / 2).SetLoops(2, LoopType.Yoyo);

                // Spawn transparent mesh of new hero shape
                GameObject transparentMesh = Instantiate(transparentPlayerMeshPrefab, transform.position, transform.rotation);
                transparentMesh.GetComponent<MeshFilter>().mesh = receivingAbility.Mesh;
                
                // Set color
                Color transparentMeshColor = hero.PlayerConfig.ColorConfig.heroMaterial.GetColor("_EmissionColor");
                transparentMeshColor.a = 0.02f;
                transparentMesh.GetComponent<MeshRenderer>().material.SetColor("_Color", transparentMeshColor);
                
                // Fade alpha
                transparentMesh.GetComponent<MeshRenderer>().material.DOFade(1f, meshFadeDuration * transmissionDuration).SetEase(Ease.InQuad);

                // Shrink into hero
                transparentMesh.transform.DOScale(1f, meshScaleDuration * transmissionDuration).SetEase(Ease.OutCubic).OnUpdate(() => 
                {
                    transparentMesh.transform.position = transform.position;
                }).OnComplete(() => 
                {
                    Destroy(transparentMesh);
                });

                // Switch abilities
                hero.SetAbility(receivingAbility);

                audioSource.PlayOneShot(transmissionSound, transmissionSoundVolume);

                transmissionPS.Play();

                // TODO: Both heroes are executing Transmit(), but only one event must be raised
                if (transmissionPartner.transmissionPartner != null) RaiseAbilitiesChanged(hero.PlayerConfig, transmissionPartner.hero.PlayerConfig);

                EndTransmission();
            });
        });
    }

    protected bool TransmissionButtonsUp()
    {
        if (Input.GetButtonUp(Constants.INPUT_TRANSMIT + hero.PlayerConfig.PlayerNumber))
        {
            return true;
        }

        return false;
    }

    protected bool TransmissionButtonsPressed()
    {
        if (Input.GetButton(Constants.INPUT_TRANSMIT + hero.PlayerConfig.PlayerNumber)) return true;

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
        hero.SetMovable(true);
        transmissionPartner = null;
        receivingAbility = null;
        ChangeState(State.Deactivated);
    }

    protected void AddReceiver(Receiver receiver)
    {
        receiver.transmitter.playerMat.DOBlendableColor(receiver.color * inRangeColorMultiplier, 0.3f);
        receivers.Add(receiver);
    }

    protected void RemoveReceiver(Receiver receiver)
    {
        receiver.transmitter.playerMat.DOBlendableColor(receiver.color, 0.3f);
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
    #endregion
}
