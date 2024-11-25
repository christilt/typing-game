using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class LevelSettingsSO : ScriptableObject
{
    [SerializeField] private string _id;
    public string LevelId => _id;
    [SerializeField] private int _order;
    public int LevelOrder => _order;
    [SerializeField] private string _sceneName;
    public string SceneName => _sceneName;  

    [SerializeField] private List<DifficultyOverride> _difficultyOverrides;

    [SerializeField] private string _namePart1;
    [SerializeField] private string _namePart2;
    [SerializeField] private string _namePartSeparator;
    public string LevelName => $"{_namePart1}{_namePartSeparator}{_namePart2}";

    [SerializeField] private Color _wallColor;
    public Color WallColor => _wallColor;


    [SerializeField] private float _benchmarkDurationSeconds;
    public float GetBenchmarkDurationSeconds(DifficultySO difficulty) => GetDifficultyValue(difficulty.Difficulty, x => x.BenchmarkDurationSeconds, _benchmarkDurationSeconds);


    [SerializeField] private CharacterSetSO _characterSet;
    public CharacterSetSO GetCharacterSet(DifficultySO difficulty) => GetDifficultyValue(difficulty.Difficulty, x => x.CharacterSet, _characterSet);


    public float GetUnitSpeedModifier(DifficultySO difficulty) => GetDifficultyValue(difficulty.Difficulty, x => x.UnitSpeedModifier, difficulty.DefaultSpeedModifier);

    public PlayerAttackSetting GetPlayerAttackSetting(DifficultySO difficulty) => GetDifficultyValue(difficulty.Difficulty, x => x.PlayerAttackSetting, difficulty.DefaultPlayerAttackSetting);

    private TValue GetDifficultyValue<TValue>(Difficulty difficulty, Func<DifficultyOverride,  TValue> overriddenValueSelector, TValue defaultValue)
    {
        var difficultyValue = _difficultyOverrides.FirstOrDefault(x => x.Difficulty == difficulty);
        if (difficultyValue == null)
            return defaultValue;

        return overriddenValueSelector(difficultyValue);
    }

    private float GetDifficultyValue(Difficulty difficulty, Func<DifficultyOverride, UnityNullableFloat> overriddenValueSelector, float defaultValue)
    {
        var difficultyValue = _difficultyOverrides.FirstOrDefault(x => x.Difficulty == difficulty);
        if (difficultyValue == null)
            return defaultValue;

        var overriddenValue = overriddenValueSelector(difficultyValue);
        if (!overriddenValue.HasValue)
            return defaultValue;

        return overriddenValue.Value;
    }

    private T GetDifficultyValue<T>(Difficulty difficulty, Func<DifficultyOverride, UnityNullableEnum<T>> overriddenValueSelector, T defaultValue) where T : Enum
    {
        var difficultyValue = _difficultyOverrides.FirstOrDefault(x => x.Difficulty == difficulty);
        if (difficultyValue == null)
            return defaultValue;

        var overriddenValue = overriddenValueSelector(difficultyValue);
        if (!overriddenValue.HasValue)
            return defaultValue;

        return overriddenValue.Value;
    }

    [Serializable]
    public class DifficultyOverride
    {
        public Difficulty Difficulty;
        public CharacterSetSO CharacterSet;
        public UnityNullableFloat BenchmarkDurationSeconds;
        public UnityNullableFloat UnitSpeedModifier;
        public UnityNullableEnum<PlayerAttackSetting> PlayerAttackSetting;
    }
}