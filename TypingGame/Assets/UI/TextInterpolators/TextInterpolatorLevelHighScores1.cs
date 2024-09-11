using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextInterpolatorLevelHighScores1 : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        return new object[]
        {
            LevelSettingsManager.Instance.LevelSettings.LevelName
        };
    }
}