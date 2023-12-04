using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Maybe split out 
[RequireComponent(typeof(BurstRecorder))]
public class TypingRecorder : MonoBehaviour
{
    [SerializeField] private int _streakRepeatsTolerated;
    [SerializeField] private int _streakRepeatsChecked;

    private BurstRecorder _burstRecorder;

    private readonly List<StreakInfo> _currentStreak = new();
    private readonly HashSet<Vector3Int> _currentStreakCheckedPositions = new(); // For O(1) lookup of repeats
    private int _currentStreakRepeats;

    private int _keysCorrect;
    private int _keysTyped;
    private int _bestStreak;
    private BurstStat _bestBurst;

    public event Action<StreakStat> OnStreakIncreased;
    public event Action OnStreakReset;

    private void Awake()
    {
        _burstRecorder = GetComponent<BurstRecorder>();
    }


    public void LogCorrectKey(KeyTile keyTile)
    {
        _keysCorrect++;
        _keysTyped++;

        if (TryValidateStreak(keyTile))
        {
            IncreaseStreak(keyTile);
            _burstRecorder.MeasureBursts(_keysCorrect, Time.time, measuredBurst =>
            {
                if (measuredBurst.IsBetterThan(_bestBurst))
                {
                    _bestBurst = measuredBurst;
                    Debug.Log($"New best burst: {measuredBurst}"); // TODO remove
                }
            });
        }
        else
        {
            ResetStreak();

            _burstRecorder.ResetBursts();
        }
    }

    public void LogIncorrectKey(KeyTile keyTile)
    {
        _keysTyped++;

        ResetStreak();
        // TODO allow some mistakes?
        _burstRecorder.ResetBursts();
    }

    public AccuracyStat CalculateAccuracy() => AccuracyStat.Calculate(_keysCorrect, _keysTyped);

    public StreakStat CalculateBestStreak() => StreakStat.Calculate(_bestStreak);
    public BurstStat CalculateTopSpeed() => _bestBurst;

    private bool TryValidateStreak(KeyTile correctKeyTile)
    {
        var isRepeat = _currentStreakCheckedPositions.TryGetValue(correctKeyTile.Position, out var _);
        if (isRepeat)
            _currentStreakRepeats++;

        return _currentStreakRepeats < _streakRepeatsTolerated;
    }

    private void IncreaseStreak(KeyTile correctKeyTile)
    {
        _currentStreak.Add(new StreakInfo(Time.time));

        _currentStreakCheckedPositions.Add(correctKeyTile.Position);
        if (_currentStreakCheckedPositions.Count > _streakRepeatsChecked)
            _currentStreakCheckedPositions.Remove(_currentStreakCheckedPositions.First());

        if (_currentStreak.Count > _bestStreak)
        {
            _bestStreak = _currentStreak.Count;
        }

        OnStreakIncreased?.Invoke(StreakStat.Calculate(_currentStreak.Count));

        //Debug.Log($"{nameof(TypingRecorder)}: Streak increased to {_currentStreak.Count}");
    }

    private void ResetStreak()
    {
        //Debug.Log($"{nameof(TypingRecorder)}: Streak reset");
        _currentStreak.Clear();
        _currentStreakCheckedPositions.Clear();
        _currentStreakRepeats = 0;

        OnStreakReset?.Invoke();
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

