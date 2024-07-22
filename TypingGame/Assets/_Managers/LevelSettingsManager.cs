using System;
using UnityEngine;

public class LevelSettingsManager : Singleton<LevelSettingsManager>
{
    [SerializeField] private LevelSettingsSO _levelSettings;
    public LevelSettingsSO LevelSettings => _levelSettings;

    public Lazy<CharacterSetSO> CharacterSet => new(() => LevelSettings.GetCharacterSet(GameSettingsManager.Instance.Difficulty));
    public Lazy<float> BenchmarkDurationSeconds => new(() => LevelSettings.GetBenchmarkDurationSeconds(GameSettingsManager.Instance.Difficulty));
    public Lazy<float> UnitSpeedModifier => new(() => LevelSettings.GetUnitSpeedModifier(GameSettingsManager.Instance.Difficulty));
    public Lazy<PlayerAttackSetting> PlayerAttackSetting => new(() => LevelSettings.GetPlayerAttackSetting(GameSettingsManager.Instance.Difficulty));
}