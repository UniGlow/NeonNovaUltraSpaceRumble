using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScore
{
    Dictionary<ScoreCategory, int> GetScore();
}
