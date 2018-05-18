using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class HeroTutorialTextUpdater : TutorialTextUpdater 
{

    #region Variable Declarations
    Hero hero;
    #endregion



    #region Unity Event Functions
    protected override void InheritedStart()
    {
        hero = player.GetComponent<Hero>();

        UpdateText();
    }
    #endregion



    #region Protected Functions
    protected override void UpdateText()
    {
        if (colorChanges <= 1)
        {
            if (hero.ability == Ability.Damage) ChangeTextTo("Damage");
            else if (hero.ability == Ability.Tank) ChangeTextTo("Tank");
            else if (hero.ability == Ability.Opfer) ChangeTextTo("Opfer");
        }
        else if (colorChanges == 2)
        {
            ChangeTextTo("Switch");
        }
        else
        {
            // If same color as Boss
            if (GameManager.Instance.Boss.WeaknessColor == PlayerColor.Blue && hero.PlayerColor == PlayerColor.Blue
                || GameManager.Instance.Boss.WeaknessColor == PlayerColor.Green && hero.PlayerColor == PlayerColor.Green
                || GameManager.Instance.Boss.WeaknessColor == PlayerColor.Red && hero.PlayerColor == PlayerColor.Red)
            {
                if (hero.ability == Ability.Damage) ChangeTextTo("DealDamage");
                else ChangeTextTo("GetDamage");
            }
            // Not Boss color
            else
            {
                if (hero.ability == Ability.Damage) ChangeTextTo("PassDamage");
                else if (hero.ability == Ability.Tank) ChangeTextTo("Tank");
                else if (hero.ability == Ability.Opfer) ChangeTextTo("Opfer");
            }
        }
    }
    #endregion
}
