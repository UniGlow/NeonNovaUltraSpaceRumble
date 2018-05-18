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
    int colorChanges;
    Hero hero;
    #endregion



    #region Unity Event Functions
    protected override void InheritedStart()
    {
        hero = player.GetComponent<Hero>();

        if (hero.ability == Ability.Damage) UpdateText("Damage");
        else if (hero.ability == Ability.Tank) UpdateText("Tank");
        else if (hero.ability == Ability.Opfer) UpdateText("Opfer");
    }

    private void Update()
    {
        transform.position = hero.transform.position + startOffset;
        transform.rotation = startRotation;
    }
    #endregion



    #region Public Functions
    public override void BossColorChanged()
    {
        colorChanges++;

        if (colorChanges == 1)
        {
            UpdateText("Switch");
        }
        else if (colorChanges >= 2)
        {
            if (GameManager.Instance.Boss.WeaknessColor == PlayerColor.Blue && hero.PlayerColor == PlayerColor.Blue) UpdateText("GetHim");
            else if (GameManager.Instance.Boss.WeaknessColor == PlayerColor.Green && hero.PlayerColor == PlayerColor.Green) UpdateText("GetHim");
            else if (GameManager.Instance.Boss.WeaknessColor == PlayerColor.Red && hero.PlayerColor == PlayerColor.Red) UpdateText("GetHim");
        }
    }
    #endregion
}
