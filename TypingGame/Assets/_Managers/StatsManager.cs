using System;
using UnityEngine;

public class StatsManager : Singleton<StatsManager>
{
    [SerializeField] private SpeedRecorder _speedRecorder;
    private LevelStats _endOfLevelStats = default;

    public AccuracyRecorder AccuracyRecorder { get; } = new();

    public LevelStats CalculateEndOfLevelStats()
    {
        if (!GameManager.Instance.State.EndsPlayerControl())
        {
            throw new InvalidOperationException("Not end of level!");
        }

        if (_endOfLevelStats != null)
        {
            return _endOfLevelStats;
        }

        _endOfLevelStats = LevelStats.Calculate(AccuracyRecorder, _speedRecorder, SettingsManager.Instance.LevelSettings);
        return _endOfLevelStats;
    }
}
