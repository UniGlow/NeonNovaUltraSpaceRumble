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
    [SerializeField] protected Color switchColor;

    [Header("References")]
    [SerializeField] protected GameObject transmissionRangeIndicator;
    [SerializeField] protected ParticleSystem transmissionPS;
    [SerializeField] protected GameEvent abilitiesChangedEvent = null;
    [SerializeField] protected GameObject shootingStarsPrefab = null;

    protected Hero hero;
    protected AudioSource audioSource;
    protected Material playerMat;
    protected State state;
    protected List<Transmission> receivers = new List<Transmission>();
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
        transmissionRangeIndicator.transform.localScale = new Vector3(transmissionRange * 0.5f, transmissionRange * 0.5f, transmissionRange * 0.5f);
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

                // Two heroes ready? => Start transmitting
                foreach (Transmission receiver in receivers)
                {
                    if (receiver.state == State.Searching || receiver.state == State.ReadyToTransmit)
                    {
                        receiver.transmissionPartner = this;
                        receiver.receivingAbility = hero.PlayerConfig.ability;
                        receiver.ChangeState(State.Transmitting);
                        transmissionPartner = receiver;
                        receivingAbility = receiver.hero.PlayerConfig.ability;
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
            receivers.Clear();
            receivers.Add(hitInfo.transform.GetComponentInParent<Transmission>());
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
        List<Collider> hits = new List<Collider>(Physics.OverlapSphere(transform.position, transmissionRange, 1 << 8));
        List<Transmission> heroesHit = new List<Transmission>();
        foreach (Collider hit in hits)
        {
            Transmission hitTransmitter = hit.transform.GetComponentInParent<Transmission>();
            if (hitTransmitter.gameObject != gameObject) heroesHit.Add(hitTransmitter);
        }

        if (heroesHit.Count > 0)
        {
            // Inform receivers that aren't any longer targeted and remove them from the list
            for (int i = receivers.Count-1; i >= 0; i--)
            {
                if (!heroesHit.Contains(receivers[i]))
                {
                    receivers.Remove(receivers[i]);
                }
            }

            // Inform new receivers and add them to the list
            foreach (Transmission hit in heroesHit)
            {
                Debug.DrawLine(transform.position + Vector3.up * 0.5f, hit.transform.position, Color.green);

                if (!receivers.Contains(hit))
                {
                    receivers.Add(hit);
                }
            }
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * transmissionRange, Color.red);
            for (int i = 0; i < 360; i += 20)
            {
                Vector3 direction = Quaternion.Euler(0, i, 0) * transform.forward;
                Debug.DrawRay(transform.position + Vector3.up * 0.5f, direction * transmissionRange, Color.red);
            }

            receivers.Clear();
        }

        return receivers.Count;
    }

    protected void Transmit()
    {
        Color originalColor = playerMat.color;
        // Blinking Hero
        playerMat.DOBlendableColor(switchColor, 0.3f).OnComplete(() => 
        {
            playerMat.DOBlendableColor(originalColor, 0.3f);
            // Shooting Stars go!
            GameObject shootingStar = Instantiate(shootingStarsPrefab, transform.position, Quaternion.LookRotation(transmissionPartner.transform.position - transform.position));
            shootingStar.transform.DOMove(transmissionPartner.transform.position, 1f).SetEase(Ease.OutSine).OnComplete(() => 
            {
                Destroy(shootingStar);
                // Blink again
                playerMat.DOBlendableColor(switchColor, 0.3f).SetLoops(2, LoopType.Yoyo);

                // TODO: Scale transparent mesh of new hero shape

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
            receivers.Clear();
        }

        if (newState == State.Transmitting)
        {
            Transmit();
        }

        state = newState;
    }

    protected void EndTransmission()
    {
        transmissionPartner = null;
        receivingAbility = null;
        ChangeState(State.Deactivated);
    }
    #endregion



    #region GameEvent Raiser
    void RaiseAbilitiesChanged(PlayerConfig hero1Config, PlayerConfig hero2Config)
    {
        abilitiesChangedEvent.Raise(this, hero1Config, hero2Config);
    }
    #endregion
}
