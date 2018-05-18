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
        ChangeTextTo("StrengthColor");
    }

    private void Update () 
	{
		
	}
    #endregion



    #region Protected Functions
    protected override void UpdateText()
    {
        
    }
    #endregion



    #region Private Functions

    #endregion
}
