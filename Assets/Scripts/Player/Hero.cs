﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
public class Hero : Character
{

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("References")]
    [SerializeField] protected GameObject shield;
    [SerializeField] protected Image cooldownIndicator;
    [SerializeField] protected MeshFilter playerMesh;
    #endregion



    #region Public Properties
    public GameObject Shield { get { return shield; } }
    public Image CooldownIndicator { get { return cooldownIndicator; } }
    public AudioSource AudioSource { get { return audioSource; } }
    public Rigidbody Rigidbody { get { return rigidbody; } }
    public MeshFilter PlayerMesh { get { return playerMesh; } }
    #endregion



    #region Unity Event Functions
    override protected void Update()
    {
        base.Update();

        if (active)
        {
            playerConfig.ability.Tick(Time.deltaTime, AbilityButtonPressed());

            // Apply class-dependant movement speed modifier
            horizontalMovement *= (1 + playerConfig.ability.SpeedBoost);
            verticalMovement *= (1 + playerConfig.ability.SpeedBoost);
        }
    }
    #endregion



    #region Public Funtcions
    public virtual void SetPlayerConfig(PlayerConfig playerConfig)
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
        playerMesh.GetComponent<Renderer>().material = playerConfig.ColorConfig.heroMaterial;
        cooldownIndicator.color = playerConfig.ColorConfig.uiElementColor;

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

        // Update Mesh
        playerMesh.mesh = ability.Mesh;
    }

    public override void ResetCooldowns()
    {
        base.ResetCooldowns();
        PlayerConfig.ability.ResetCooldowns(true);
    }
    #endregion



    #region Private Functions
    bool AbilityButtonPressed()
    {
        if (playerConfig.ability.Autofire)
        {
            if (playerConfig.Player.GetButton(RewiredConsts.Action.TRIGGER_HEROABILITY))
                return true;
        }
        else
        {
            if (playerConfig.Player.GetButtonDown(RewiredConsts.Action.TRIGGER_HEROABILITY))
                return true;
        }

        return false;
    }
    #endregion
}
