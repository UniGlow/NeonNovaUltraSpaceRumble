using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Score/HeroScore")]
public class HeroScore : ScriptableObject, IScore 
{
    LinkedList<LevelScore> levelScores = new LinkedList<LevelScore>();

    public LevelScore CurrentLevelScore { get { return levelScores.Last.Value; } }



    public int GetScore()
    {
        throw new System.NotImplementedException();
    }
}
