using System;
using UnityEngine;

public class StatsManager : Singleton<StatsManager>
{
    [SerializeField] private SpeedRecorder _speedRecorder;
    [SerializeField] private int _streakRepeatsTolerated;
    [SerializeField] private int _streakRepeatsChecked;
    private LevelStats _endOfLevelStats = default;

    protected override void Awake()
    {
        base.Awake();
        TypingRecorder =  new(_streakRepeatsTolerated, _streakRepeatsChecked);
    }

    public TypingRecorder TypingRecorder { get; private set; }

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

        _endOfLevelStats = LevelStats.Calculate(TypingRecorder, _speedRecorder, SettingsManager.Instance.LevelSettings);
        return _endOfLevelStats;
    }
}
