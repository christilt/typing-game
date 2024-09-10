﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextInterpolatorLevelHighScores1 : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        var levelId = LevelSettingsManager.Instance.LevelSettings.LevelId;
        var levelHighScores = SaveDataManager.Instance.LoadLevelHighScores(levelId);

        var whiteColourString = ColorUtility.ToHtmlStringRGB(Color.white);
        var argsList = new List<object>();
        argsList.Add(LevelSettingsManager.Instance.LevelSettings.LevelName);
        for (var i = 0; i < 5; i++)
        {
            if (levelHighScores.Count > i)
            {
                var highScore = levelHighScores[i];
                argsList.Add(highScore.Initials);
                argsList.Add(highScore.ColourHtmlString);
                argsList.Add(highScore.Value);
            }
            else
            {
                argsList.Add("...");
                argsList.Add(whiteColourString);
                argsList.Add("...");
            }
        }

        return argsList.ToArray();
    }
}