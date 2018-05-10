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
    [SerializeField] protected Material greenBossMat;
    [SerializeField] protected Material redBossMat;
    [SerializeField] protected Material blueBossMat;

    Boss boss;
    PlayerColor weaknessColor;
    PlayerColor strengthColor;

    public PlayerColor WeaknessColor { get { return weaknessColor; } }
    public PlayerColor StrengthColor { get { return strengthColor; } }
    #endregion



    #region Unity Event Functions
    private void Start()
    {
        boss = GameManager.Instance.Boss;
        weaknessColor = boss.WeaknessColor;
        SetMaterialColor();
        strengthColor = boss.StrengthColor;
    }

    private void Update()
    {
        if (weaknessColor != boss.WeaknessColor)
        {
            weaknessColor = boss.WeaknessColor;
            SetMaterialColor();
        }

        if (strengthColor != boss.StrengthColor)
        {
            strengthColor = boss.StrengthColor;
        }
    }
    #endregion



    #region Private Functions
    void SetMaterialColor()
    {
        if (weaknessColor == PlayerColor.Green) meshRenderer.material = greenBossMat;
        if (weaknessColor == PlayerColor.Blue) meshRenderer.material = blueBossMat;
        if (weaknessColor == PlayerColor.Red) meshRenderer.material = redBossMat;
    }
    #endregion
}
