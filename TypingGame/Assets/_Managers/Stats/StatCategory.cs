using UnityEngine;

public enum StatCategory
{
    Great,
    Good,
    Average,
    Bad
}

public static class StatCategoryExtensions
{
    public static Color GetColour(this StatCategory category)
    {
        switch (category)
        {
            case StatCategory.Great:
                return GameSettingsManager.Instance.Palette.GreatColour;
            case StatCategory.Good:
                return GameSettingsManager.Instance.Palette.GoodColour;
            case StatCategory.Average:
                return GameSettingsManager.Instance.Palette.AverageColour;
            default:
                return GameSettingsManager.Instance.Palette.BadColour;
        }
    }

    public static bool IsAtLeastGood(this StatCategory category)
    {
        return category == StatCategory.Good
            || category == StatCategory.Great;
    }
}