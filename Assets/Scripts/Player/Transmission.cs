using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Hero))]
public class Transmission : MonoBehaviour {

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
    public ParticleSystem TransmissionPS { get { return transmissionPS; } }

    protected Hero hero;
    protected GameObject receiver;
    protected bool receiverFound = false;
    protected float currenTransmissionDuration;
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
        if (Input.GetButtonUp(Constants.INPUT_TRANSMIT + hero.PlayerNumber)) {
            EndTransmission();
        }

        // Look for a receiver
        if (!receiverFound && transmissionCooldownB && Input.GetButton(Constants.INPUT_TRANSMIT + hero.PlayerNumber)) {
            UpdateLineRenderer();
            transmissionLineRenderer.gameObject.SetActive(true);
            receiverFound = FindReceiver();
        }
        // Continue the transmission when a receiver is found
        else if (receiverFound && Input.GetButton(Constants.INPUT_TRANSMIT + hero.PlayerNumber)) {
            UpdateLineRenderer();
            Transmit();
        }

        
    }
    #endregion



    public void EndTransmission()
    {
        receiver = null;
        currenTransmissionDuration = 0f;
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
        else {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * transmissionRange, Color.red);
            return false;
        }

    }

    void Transmit()
    {
        // End transmission if out of range
        if ((transform.position - receiver.transform.position).magnitude > transmissionRange) {
            EndTransmission();
            return;
        }

        currenTransmissionDuration += Time.deltaTime;
        Debug.DrawLine(transform.position, receiver.transform.position, Color.green);

        // Successfull transmission: Swap abilities and end transmission
        if (currenTransmissionDuration >= transmissionDuration) {
            Hero otherHero = receiver.GetComponent<Hero>();
            Ability newAbility = otherHero.ability;

            // Cancel defend cooldown, if one of the abilities was Tank
            if (hero.ability == Ability.Tank) hero.CancelDefendReset();
            else if (otherHero.ability == Ability.Tank) otherHero.CancelDefendReset();

            // Switch abilities
            otherHero.ability = hero.ability;
            hero.ability = newAbility;

            // Set the new Ability Sprite for this hero
            if (hero.ability == Ability.Damage) hero.CooldownIndicator.sprite = hero.DamageSprite;
            else if (hero.ability == Ability.Opfer) hero.CooldownIndicator.sprite = hero.OpferSprite;
            else if (hero.ability == Ability.Tank) hero.CooldownIndicator.sprite = hero.TankSprite;

            // Set the new Ability Sprite for the other hero
            if (otherHero.ability == Ability.Damage) otherHero.CooldownIndicator.sprite = otherHero.DamageSprite;
            else if (otherHero.ability == Ability.Opfer) otherHero.CooldownIndicator.sprite = otherHero.OpferSprite;
            else if (otherHero.ability == Ability.Tank) otherHero.CooldownIndicator.sprite = otherHero.TankSprite;

            audioSource.PlayOneShot(transmissionSound, transmissionSoundVolume);

            homingMissile.AcquireNewTarget();
            if (GameManager.Instance.GetActiveSceneName().Contains("Tutorial")) TutorialTextUpdater.UpdateTexts();

            transmissionPS.Play();
            receiver.GetComponent<Transmission>().transmissionPS.Play();
            receiver.GetComponent<Transmission>().EndTransmission();

            EndTransmission();
        }
    }

    protected void UpdateLineRenderer() {
        if (receiver == null) {
            transmissionLineRenderer.SetPosition(0, transform.position + Vector3.up * 0.5f);
            transmissionLineRenderer.SetPosition(1, transform.position + Vector3.up * 0.5f + transform.forward * transmissionRange);
        }
        else {
            transmissionLineRenderer.SetPosition(0, transform.position + Vector3.up * 0.5f);
            transmissionLineRenderer.SetPosition(1, receiver.transform.position + Vector3.up * 0.5f);
        }
    }
    #endregion



    protected IEnumerator ResetTransmissionCooldown()
    {
        yield return new WaitForSecondsRealtime(transmissionCooldown);
        transmissionCooldownB = true;
    }
}
