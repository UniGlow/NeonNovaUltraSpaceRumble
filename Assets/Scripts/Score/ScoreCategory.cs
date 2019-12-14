using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Score/Score Category")]
public class ScoreCategory : ScriptableObject
{
    public new string name;
    public string displayName;
    public float optimalValue;
    public float worstValue;
}
