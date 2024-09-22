using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextInterpolatorLevelHighScoresRowNew : TextInterpolator
{
    private int _index;
    private List<HighScore> _highScores;
    private string _whiteColourString;

    public void Enable(int index, List<HighScore> highScores, string whiteColourString)
    {
        _index = index;
        _highScores = highScores;
        _whiteColourString = whiteColourString;
        gameObject.SetActive(true);
    }

    protected override object[] GetTextArgs()
    {
        var argsList = new List<object>();
        var highScore = _highScores[_index];
        argsList.Add(_index + 1);
        //argsList.Add(SaveDataManager.Instance.LoadLastPlayerInitials()); // Blank for entry by input field
        argsList.Add(highScore.RankColourHtmlString);
        argsList.Add(highScore.Score);
        argsList.Add(highScore.TimeColourHtmlString);
        argsList.Add(new TimeSpan(0, highScore.Minutes, highScore.Seconds));

        return argsList.ToArray();
    }
}