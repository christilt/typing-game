using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StreakRecorder : MonoBehaviour
{
    [SerializeField] private int _notifyAfterMinimumCount;
    [SerializeField] private int _notifyAfterIntervalCount;

    private readonly List<StreakInfo> _currentStreak = new();
    
    private int _bestStreak;

    public event Action<StreakStat> OnStreakIncreased;
    public event Action<StreakStat> OnStreakNotification;
    public event Action OnStreakReset;

    public StreakStat CalculateBestStreak() => StreakStat.Calculate(_bestStreak);

    public void LogValidKey()
    {
        _currentStreak.Add(new StreakInfo(Time.time));

        if (_currentStreak.Count > _bestStreak)
        {
            _bestStreak = _currentStreak.Count;
        }

        var streakStat = StreakStat.Calculate(_currentStreak.Count);
        OnStreakIncreased?.Invoke(streakStat);
        MaybeNotify(streakStat);

        //Debug.Log($"{nameof(TypingRecorder)}: Streak increased to {_currentStreak.Count}");
    }

    public void LogIncorrectKey()
    {
        ResetStreak();
    }

    public void ResetStreak()
    {
        _currentStreak.Clear();

        OnStreakReset?.Invoke();
    }

    private void MaybeNotify(StreakStat streak)
    {
        if (streak.Count < _notifyAfterMinimumCount)
            return;

        if (streak.Count % _notifyAfterIntervalCount != 0)
            return;

        OnStreakNotification?.Invoke(streak);
    }

    private struct StreakInfo
    {
        public StreakInfo(float time)
        {
            Time = time;
        }

        public float Time { get; }
    }
}

