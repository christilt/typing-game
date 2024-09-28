using System;
using UnityEngine;

public class StatsManager : Singleton<StatsManager>
{
    [SerializeField] private SpeedRecorder _speedRecorder;
    [SerializeField] private TypingRecorder _typingRecorder;
    private LevelStats _endOfLevelStats = default;

    public TypingRecorder TypingRecorder => _typingRecorder;

    public LevelStats CalculateEndOfLevelStats()
    {
        if (!GameplayManager.Instance.State.EndsGameplay())
        {
            throw new InvalidOperationException("Not end of level!");
        }

        if (_endOfLevelStats != null)
        {
            return _endOfLevelStats;
        }

        _endOfLevelStats = LevelStats.Calculate(TypingRecorder, _speedRecorder);
        return _endOfLevelStats;
    }
}
