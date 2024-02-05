public enum PlayerAttackSetting
{
    None,
    StartEmpty,
    StartFull
}

public static class PlayerAttackSettingExtensions
{
    public static bool IsDisplayable(this PlayerAttackSetting playerAttackSetting)
    {
        return playerAttackSetting != PlayerAttackSetting.None;
    }
}