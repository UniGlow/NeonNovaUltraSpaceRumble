using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BossTutorialTextUpdater : TutorialTextUpdater 
{

    #region Variable Declarations

    #endregion



    #region Unity Event Functions
    protected override void InheritedStart()
    {
        UpdateText();
    }
    #endregion



    #region Protected Functions
    protected override void UpdateText()
    {
        if (colorChanges <= 1) ChangeTextTo("");
        else if (colorChanges >= 2 && colorChanges % 2 == 0) ChangeTextTo("WeaknessColor");
        else if (colorChanges >= 3 && colorChanges % 2 == 1) ChangeTextTo("StrengthColor");
    }
    #endregion



    #region Private Functions

    #endregion
}
