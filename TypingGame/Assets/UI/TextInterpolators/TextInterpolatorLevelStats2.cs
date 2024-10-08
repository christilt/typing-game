﻿using UnityEngine;

public class TextInterpolatorLevelStats2 : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();

        return new object[]
        {
            ColorUtility.ToHtmlStringRGBA(stats.Accuracy.Category.GetColour()),
            stats.Accuracy.Proportion,

            ColorUtility.ToHtmlStringRGBA(stats.Speed.Category.GetColour()),
            stats.Speed.TimeTaken,

            ColorUtility.ToHtmlStringRGBA(stats.BestStreak.Category.GetColour()),
            stats.BestStreak.Count,

            ColorUtility.ToHtmlStringRGBA(stats.TopSpeed.Category.GetColour()),
            $"{stats.TopSpeed.WordsPerMinute} WPM",
        };
    }
}