using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextInterpolatorLevelHighScoresHeader : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        var difficulty = GameSettingsManager.Instance.Difficulty;
        var argsList = new List<object>();
        argsList.Add(difficulty.Difficulty == Difficulty.Normal ? string.Empty : $"{difficulty.Name} mode");

        return argsList.ToArray();
    }
}