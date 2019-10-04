using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Hero))]
public class Transmission : MonoBehaviour
{

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

    [Header("References")]
    [SerializeField] protected LineRenderer transmissionLineRenderer;
    [SerializeField] protected ParticleSystem transmissionPS;
    [SerializeField] protected GameEvent abilitiesChangedEvent = null;

    protected Hero hero;
    protected GameObject receiver;
    protected float currentTransmissionDuration;
    protected bool transmissionReady = true;
    protected AudioSource audioSource;
    GameObject targetedBy;
    bool endingTransmission;
    #endregion



    #region Public Properties
    public ParticleSystem TransmissionPS { get { return transmissionPS; } }
    public GameObject TargetedBy { get { return targetedBy; } set { targetedBy = value; } }
    #endregion



    #region Unity Event Functions
    virtual protected void Start()
    {
        hero = GetComponent<Hero>();
        audioSource = GetComponent<AudioSource>();
	}
	
	virtual protected void Update()
    {        
        // End transmission if button is lifted
        if (TransmissionButtonsUp())
        {
            EndTransmission();
        }

        // Look for a receiver
        if (receiver == null && transmissionReady && TransmissionButtonsPressed())
        {
            UpdateLineRenderer();
            transmissionLineRenderer.gameObject.SetActive(true);
            FindReceiverCircle();
        }
        // Continue the transmission when a receiver is found
        else if (receiver != null && TransmissionButtonsPressed())
        {
            UpdateLineRenderer();
            Transmit();
        }
    }
    #endregion

    

    public void EndTransmission()
    {
        endingTransmission = true;

        if (receiver != null && !receiver.GetComponent<Transmission>().endingTransmission)
        {
            receiver.GetComponent<Transmission>().EndTransmission();
            receiver = null;
        }
        targetedBy = null;
        currentTransmissionDuration = 0f;
        transmissionReady = false;
        transmissionLineRenderer.gameObject.SetActive(false);
        StartCoroutine(ResetTransmissionCooldown());

        endingTransmission = false;
    }



    #region Private Functions
    bool FindReceiverRay()
    {
        RaycastHit hitInfo;
        // Send 3 rays for a casual aiming
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hitInfo, transmissionRange, 1 << 8)
            || Physics.Raycast(transform.position + Vector3.up * 0.5f + Vector3.right * 0.3f, transform.forward, out hitInfo, transmissionRange, 1 << 8)
            || Physics.Raycast(transform.position + Vector3.up * 0.5f - Vector3.right * 0.3f, transform.forward, out hitInfo, transmissionRange, 1 << 8))
        {
            Debug.DrawLine(transform.position + Vector3.up * 0.5f, hitInfo.point, Color.green);
            receiver = hitInfo.transform.gameObject;
            receiver.GetComponent<Transmission>().TargetedBy = gameObject;
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * transmissionRange, Color.red);
            return false;
        }
    }

    bool FindReceiverCircle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, transmissionRange, 1 << 8);

        if (hits.Length > 1)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponentInParent<Rigidbody>().gameObject != gameObject)
                {
                    Debug.DrawLine(transform.position + Vector3.up * 0.5f, hits[i].transform.position, Color.green);
                    receiver = hits[i].transform.GetComponentInParent<Rigidbody>().gameObject;
                    receiver.GetComponent<Transmission>().TargetedBy = gameObject;
                    return true;
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
        }

        return false;
    }

    protected void Transmit()
    {
        // End transmission if out of range
        if ((transform.position - receiver.transform.position).magnitude > transmissionRange + 2f)
        {
            EndTransmission();
            return;
        }

        currentTransmissionDuration += Time.deltaTime;
        Debug.DrawLine(transform.position, receiver.transform.position, Color.green);

        // Successfull transmission: Swap abilities and end transmission
        if (targetedBy == receiver)
        {
            Hero otherHero = receiver.GetComponent<Hero>();
            Ability newAbility = otherHero.PlayerConfig.ability;

            // Switch abilities
            otherHero.SetAbility(hero.PlayerConfig.ability);
            hero.SetAbility(newAbility);

            audioSource.PlayOneShot(transmissionSound, transmissionSoundVolume);

            RaiseAbilitiesChanged(hero.PlayerConfig, otherHero.PlayerConfig);

            transmissionPS.Play();
            receiver.GetComponent<Transmission>().transmissionPS.Play();

            EndTransmission();
        }
    }

    protected void UpdateLineRenderer()
    {
        if (receiver == null)
        {
            transmissionLineRenderer.SetPosition(0, transform.position + Vector3.up * 0.5f);
            transmissionLineRenderer.SetPosition(1, transform.position + Vector3.up * 0.5f + transform.forward * transmissionRange);
        }
        else
        {
            transmissionLineRenderer.SetPosition(0, transform.position + Vector3.up * 0.5f);
            transmissionLineRenderer.SetPosition(1, receiver.transform.position + Vector3.up * 0.5f);
        }
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
    #endregion



    #region GameEvent Raiser
    void RaiseAbilitiesChanged(PlayerConfig hero1Config, PlayerConfig hero2Config)
    {
        abilitiesChangedEvent.Raise(this, hero1Config, hero2Config);
    }
    #endregion



    protected IEnumerator ResetTransmissionCooldown()
    {
        yield return new WaitForSecondsRealtime(transmissionCooldown);
        transmissionReady = true;
    }
}
