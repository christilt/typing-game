using UnityEngine;

public class TextInterpolatorLevelStats2 : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();

        return new object[]
        {
            ColorUtility.ToHtmlStringRGB(stats.Accuracy.Category.GetColour()),
            stats.Accuracy.Proportion,

            ColorUtility.ToHtmlStringRGB(stats.Speed.Category.GetColour()),
            stats.Speed.TimeTaken,

            ColorUtility.ToHtmlStringRGB(stats.BestStreak.Category.GetColour()),
            stats.BestStreak.Count,

            ColorUtility.ToHtmlStringRGB(stats.TopSpeed.Category.GetColour()),
            $"{stats.TopSpeed.WordsPerMinute} WPM",
        };
    }
}