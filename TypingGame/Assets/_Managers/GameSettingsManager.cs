using System;
using UnityEngine;

public class GameSettingsManager : PersistentSingleton<GameSettingsManager>
{
    [SerializeField] private DifficultySO _defaultDifficulty;
    private DifficultySO _difficulty;
    public DifficultySO Difficulty
    {
        get => _difficulty ?? _defaultDifficulty;
        set
        {
            _difficulty = value;
            OnDifficultyChanged?.Invoke(value);
        }
    }

    public event Action<DifficultySO> OnDifficultyChanged;

    [SerializeField] private PaletteSO _palette;
    public PaletteSO Palette => _palette;


    [SerializeField] private MenuTransitionsSO _menuTransitions;
    public MenuTransitionsSO MenuTransitions => _menuTransitions;
}