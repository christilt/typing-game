using UnityEngine;

public class TextInterpolatorLevelWon2 : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();
        var color = stats.Category.GetColour();
        var colorString = ColorUtility.ToHtmlStringRGB(color);

        return new object[]
        {
            colorString,
            stats.Rank.ToString()
        };
    }
}