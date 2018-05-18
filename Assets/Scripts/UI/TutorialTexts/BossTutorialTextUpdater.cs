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
        UpdateText("StrengthColor");
    }

    private void Update () 
	{
		
	}
    #endregion



    #region Public Functions
    public override void BossColorChanged()
    {
        
    }
    #endregion



    #region Private Functions

    #endregion
}
