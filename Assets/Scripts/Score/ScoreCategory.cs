using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Score/Score Category")]
public class ScoreCategory : ScriptableObject
{
    public string name;
    public float optimalValue;
    public Ability.AbilityClass abilityClass;
}
