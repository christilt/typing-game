using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class LevelSettingsSO : ScriptableObject
{
    [SerializeField] private List<DifficultyOverride> _difficultyOverrides;

    [SerializeField] private string _namePart1;
    [SerializeField] private string _namePart2;
    [SerializeField] private string _namePartSeparator;
    public string LevelName => $"{_namePart1}{_namePartSeparator}{_namePart2}";


    [SerializeField] private float _benchmarkDurationSeconds;
    public float GetBenchmarkDurationSeconds(Difficulty difficulty) => GetDifficultyValue(difficulty, x => x.BenchmarkDurationSeconds, _benchmarkDurationSeconds);


    [SerializeField] private CharacterSetSO _characterSet;
    public CharacterSetSO GetCharacterSet(Difficulty difficulty) => GetDifficultyValue(difficulty, x => x.CharacterSet, _characterSet);


    public float GetUnitSpeedModifier(Difficulty difficulty) => GetDifficultyValue(difficulty, x => x.UnitSpeedModifier, 1);





    private TValue GetDifficultyValue<TValue>(Difficulty difficulty, Func<DifficultyOverride, TValue> overriddenValueSelector, TValue defaultValue)
    {
        var difficultyValue = _difficultyOverrides.FirstOrDefault(x => x.Difficulty == difficulty);

        if (difficultyValue != null)
        {
            return overriddenValueSelector(difficultyValue);
        }

        return defaultValue;
    }

    private float GetDifficultyValue(Difficulty difficulty, Func<DifficultyOverride, UnityNullableFloat> overriddenValueSelector, float defaultValue)
    {
        var difficultyValue = _difficultyOverrides.FirstOrDefault(x => x.Difficulty == difficulty);

        if (difficultyValue != null)
        {
            var overriddenValue = overriddenValueSelector(difficultyValue);
            if (overriddenValue.HasValue)
                return overriddenValue.Value;
        }

        return defaultValue;
    }

    [Serializable]
    public class DifficultyOverride
    {
        public Difficulty Difficulty;
        public CharacterSetSO CharacterSet;
        public UnityNullableFloat BenchmarkDurationSeconds;
        public UnityNullableFloat UnitSpeedModifier;
    }
}