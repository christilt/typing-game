﻿using UnityEngine;

public class TextInterpolatorLevelStats1 : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();

        return new object[]
        {
            LevelSettingsManager.Instance.LevelSettings.LevelName,

            ColorUtility.ToHtmlStringRGB(stats.Category.GetColour()),
            stats.Rank.ToString(),

            stats.Score.ToString(),
        };
    }
}