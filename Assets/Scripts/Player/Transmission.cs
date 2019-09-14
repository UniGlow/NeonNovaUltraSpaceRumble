using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Hero))]
public class Transmission : MonoBehaviour
{

    #region Variable Declarations
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
    public ParticleSystem TransmissionPS { get { return transmissionPS; } }

    protected Hero hero;
    protected GameObject receiver;
    protected bool receiverFound = false;
    protected float currentTransmissionDuration;
    protected bool transmissionCooldownB = true;
    protected HomingMissile homingMissile;
    protected AudioSource audioSource;
	#endregion
	
	
	
	#region Unity Event Functions
	virtual protected void Start()
    {
        hero = GetComponent<Hero>();
        homingMissile = GameObject.FindObjectOfType<HomingMissile>();
        audioSource = GetComponent<AudioSource>();
	}
	
	virtual protected void Update()
    {        
        // End transmission if button is lifted
        if (hero.PlayerConfig.Player.GetButtonUp(RewiredConsts.Action.TRANSMIT_ABILITY))
        {
            EndTransmission();
        }

        // Look for a receiver
        if (!receiverFound && transmissionCooldownB && hero.PlayerConfig.Player.GetButton(RewiredConsts.Action.TRANSMIT_ABILITY))
        {
            UpdateLineRenderer();
            transmissionLineRenderer.gameObject.SetActive(true);
            receiverFound = FindReceiver();
        }
        // Continue the transmission when a receiver is found
        else if (receiverFound && hero.PlayerConfig.Player.GetButton(RewiredConsts.Action.TRANSMIT_ABILITY))
        {
            UpdateLineRenderer();
            Transmit();
        }
    }
    #endregion



    public void EndTransmission()
    {
        receiver = null;
        currentTransmissionDuration = 0f;
        receiverFound = false;
        transmissionCooldownB = false;
        transmissionLineRenderer.gameObject.SetActive(false);
        StartCoroutine(ResetTransmissionCooldown());
    }



    #region Private Functions
    bool FindReceiver()
    {
        RaycastHit hitInfo;
        // Send 3 rays for a casual aiming
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hitInfo, transmissionRange, 1 << 8)
            || Physics.Raycast(transform.position + Vector3.up * 0.5f + Vector3.right * 0.3f, transform.forward, out hitInfo, transmissionRange, 1 << 8)
            || Physics.Raycast(transform.position + Vector3.up * 0.5f - Vector3.right * 0.3f, transform.forward, out hitInfo, transmissionRange, 1 << 8))
        {
            Debug.DrawLine(transform.position + Vector3.up * 0.5f, hitInfo.point, Color.green);
            receiver = hitInfo.transform.gameObject;
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * transmissionRange, Color.red);
            return false;
        }

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
        if (currentTransmissionDuration >= transmissionDuration)
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
            receiver.GetComponent<Transmission>().EndTransmission();

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
        transmissionCooldownB = true;
    }
}
