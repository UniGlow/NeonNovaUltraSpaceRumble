using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScore
{
    List<ScoreCategoryResult> GetScore();
}

[System.Serializable]
public class ScoreCategoryResult
{
    public ScoreCategory scoreCategory;
    public int result;

    public ScoreCategoryResult(ScoreCategory scoreCategory, int result)
    {
        this.scoreCategory = scoreCategory;
        this.result = result;
    }
}