using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
public class Hero : Character
{

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("References")]
    [SerializeField] protected GameObject wobbleBobble;
    public SpriteRenderer healthIndicator;
    [SerializeField] protected SpriteRenderer cooldownIndicator;
    [SerializeField] protected Sprite[] defendCooldownSprites;
    [SerializeField] protected Sprite damageSprite;
    [SerializeField] protected Sprite opferSprite;
    [SerializeField] protected Renderer playerMeshRenderer;
    #endregion



    #region Public Properties
    public GameObject WobbleBobble { get { return wobbleBobble; } }
    public SpriteRenderer CooldownIndicator { get { return cooldownIndicator; } }
    public Sprite TankSprite { get { return defendCooldownSprites[defendCooldownSprites.Length - 1]; } }
    public Sprite DefendCooldownSprite { get { return defendCooldownSprites[0]; } }
    public Sprite[] DefendCooldownSprites { get { return defendCooldownSprites; } }
    public Sprite DamageSprite { get { return damageSprite; } }
    public Sprite OpferSprite { get { return opferSprite; } }
    public AudioSource AudioSource { get { return audioSource; } }
    public Rigidbody Rigidbody { get { return rigidbody; } }
    #endregion



    #region Unity Event Functions
    override protected void Update()
    {
        base.Update();

        if (active)
        {
            playerConfig.ability.Tick(Time.deltaTime, playerConfig.Player.GetButtonDown(RewiredConsts.Action.TRIGGER_HEROABILITY));

            // Apply class-dependant movement speed modifier
            horizontalMovement *= (1 + playerConfig.ability.SpeedBoost);
            verticalMovement *= (1 + playerConfig.ability.SpeedBoost);
        }
    }
    #endregion



    #region Public Funtcions
    public void SetPlayerConfig(PlayerConfig playerConfig)
    {
        this.playerConfig = playerConfig;

        playerConfig.Player.controllers.maps.layoutManager.ruleSets.Clear();
        if (playerConfig.Faction == Faction.Heroes)
        {
            playerConfig.Player.controllers.maps.layoutManager.ruleSets.Add(ReInput.mapping.GetControllerMapLayoutManagerRuleSetInstance("RuleSetHero"));
            PlayerConfig.Player.controllers.maps.layoutManager.Apply();
        }
        else Debug.LogError("Hero's playerConfig has set a wrong Faction.", this);

        // Set colors
        playerMeshRenderer.material = playerConfig.ColorConfig.heroMaterial;
        cooldownIndicator.color = playerConfig.ColorConfig.uiElementColor;
        healthIndicator.color = playerConfig.ColorConfig.uiElementColor;

        // TODO
        SetAbility(playerConfig.ability);
    }

    public void SetAbility(Ability ability)
    {
        // Cancel shield, if current ability is Tank class
        if (playerConfig.ability.Binded && playerConfig.ability.Class == Ability.AbilityClass.Tank)
        {
            Tank tankAbility = playerConfig.ability as Tank;
            tankAbility.DeactivateShield();
        }

        // Set new ability
        playerConfig.ability = ability;
        ability.BindTo(this);

        // Update class sprites
        switch (ability.Class)
        {
            case Ability.AbilityClass.Damage:
                cooldownIndicator.sprite = DamageSprite;
                break;
            case Ability.AbilityClass.Tank:
                cooldownIndicator.sprite = TankSprite;
                break;
            case Ability.AbilityClass.Victim:
                cooldownIndicator.sprite = opferSprite;
                break;
            default:
                break;
        }
    }
    #endregion



    #region Private Functions

    #endregion
}
