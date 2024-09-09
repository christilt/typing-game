public class TextInterpolatorLevelWon1 : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        return new object[]
        {
            LevelSettingsManager.Instance.LevelSettings.LevelName
        };
    }
}