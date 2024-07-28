using UnityEngine;

public class GameSettingsManager : PersistentSingleton<GameSettingsManager>
{
    [SerializeField] private DifficultySO _defaultDifficulty;
    private DifficultySO _difficulty;
    public DifficultySO Difficulty
    {
        get => _difficulty ?? _defaultDifficulty;
        set => _difficulty = value;
    }

    [SerializeField] private PaletteSO _palette;
    public PaletteSO Palette => _palette;
}