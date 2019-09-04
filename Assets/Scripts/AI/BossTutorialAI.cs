using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BossTutorialAI : MonoBehaviour 
{

    #region Variable Declarations
    [SerializeField] MeshRenderer meshRenderer;

    Boss boss;
    PlayerColor2 weaknessColor;

    public PlayerColor2 WeaknessColor { get { return weaknessColor; } }
    #endregion


    
    #region Unity Event Functions

    #endregion



    #region Public Functions
    public void ChangeColor(PlayerConfig bossConfig)
    {
        weaknessColor = bossConfig.ColorConfig;
        SetMaterialColor();
    }
    #endregion



    #region Private Functions
    void SetMaterialColor()
    {
        meshRenderer.material = weaknessColor.bossMaterial;
    }
    #endregion
}
