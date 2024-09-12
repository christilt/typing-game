using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextInterpolatorLevelHighScores2 : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        var levelId = LevelSettingsManager.Instance.LevelSettings.LevelId;
        var levelHighScores = SaveDataManager.Instance.LoadLevelHighScores(levelId);

        var whiteColourString = ColorUtility.ToHtmlStringRGB(Color.white);
        var argsList = new List<object>();
        for (var i = 0; i < 5; i++)
        {
            if (levelHighScores.Count > i)
            {
                var highScore = levelHighScores[i];
                argsList.Add(i + 1);
                argsList.Add(highScore.Initials);
                argsList.Add(highScore.RankColourHtmlString);
                argsList.Add(highScore.Score);
                argsList.Add(highScore.TimeColourHtmlString);
                argsList.Add(new TimeSpan(0, highScore.Minutes, highScore.Seconds));
            }
            else
            {
                argsList.Add("..");
                argsList.Add("...");
                argsList.Add(whiteColourString);
                argsList.Add("...");
                argsList.Add(whiteColourString);
                argsList.Add("...");
            }
        }

        return argsList.ToArray();
    }
}