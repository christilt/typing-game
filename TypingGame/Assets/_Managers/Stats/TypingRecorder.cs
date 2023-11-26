using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TypingRecorder
{
    private readonly int _streakRepeatsTolerated;
    private readonly int _streakRepeatsChecked;
    private readonly List<KeyTile> _currentStreak = new();
    private readonly HashSet<Vector3Int> _currentStreakCheckedPositions = new(); // For O(1) lookup of repeats
    private int _currentStreakRepeats;

    public TypingRecorder(int streakRepeatsTolerated, int streakRepeatsChecked)
    {
        _streakRepeatsTolerated = streakRepeatsTolerated;
        _streakRepeatsChecked = streakRepeatsChecked;
    }

    public event Action<int> OnStreakIncreased;

    public int KeysCorrect { get; private set; }
    public int KeysTyped { get; private set; }

    public void LogCorrectKey(KeyTile keyTile)
    {
        KeysCorrect++;
        KeysTyped++;

        if (TryValidateStreak(keyTile))
        {
            IncreaseStreak(keyTile);
        }
        else
        {
            ResetStreak();
        }
    }

    public void LogIncorrectKey(KeyTile keyTile)
    {
        KeysTyped++;

        ResetStreak();
    }

    public AccuracyStat CalculateAccuracy() => AccuracyStat.Calculate(KeysCorrect, KeysTyped);

    private bool TryValidateStreak(KeyTile correctKeyTile)
    {
        var isRepeat = _currentStreakCheckedPositions.TryGetValue(correctKeyTile.Position, out var _);
        if (isRepeat)
            _currentStreakRepeats++;

        return _currentStreakRepeats < _streakRepeatsTolerated;
    }

    private void IncreaseStreak(KeyTile correctKeyTile)
    {
        _currentStreak.Add(correctKeyTile);

        _currentStreakCheckedPositions.Add(correctKeyTile.Position);
        if (_currentStreakCheckedPositions.Count > _streakRepeatsChecked)
            _currentStreakCheckedPositions.Remove(_currentStreakCheckedPositions.First());

        OnStreakIncreased?.Invoke(_currentStreak.Count);

        //Debug.Log($"{nameof(TypingRecorder)}: Streak increased to {_currentStreak.Count}");
    }

    private void ResetStreak()
    {
        //Debug.Log($"{nameof(TypingRecorder)}: Streak reset");
        _currentStreak.Clear();
        _currentStreakCheckedPositions.Clear();
        _currentStreakRepeats = 0;
    }
}
