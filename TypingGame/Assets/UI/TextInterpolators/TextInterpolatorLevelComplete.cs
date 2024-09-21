public class TextInterpolatorLevelComplete : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        return new object[]
        {
            LevelSettingsManager.Instance.LevelSettings.LevelName
        };
    }
}