using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    // TODO: Have this selected instead of serialised
    [SerializeField] private DifficultySO _difficulty;
    public DifficultySO Difficulty => _difficulty;

    [SerializeField] private LevelSettingsSO _levelSettings;
    public LevelSettingsSO LevelSettings => _levelSettings;

    [SerializeField] private PaletteSO _palette;
    public PaletteSO Palette => _palette;

    public Lazy<CharacterSetSO> CharacterSet => new(() => LevelSettings.GetCharacterSet(Difficulty));
    public Lazy<float> BenchmarkDurationSeconds => new(() => LevelSettings.GetBenchmarkDurationSeconds(Difficulty));
    public Lazy<float> UnitSpeedModifier => new(() => LevelSettings.GetUnitSpeedModifier(Difficulty));
}