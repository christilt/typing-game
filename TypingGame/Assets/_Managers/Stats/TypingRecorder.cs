using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(StreakRecorder), typeof(BurstRecorder))]
public class TypingRecorder : MonoBehaviour
{
    [SerializeField] private int _repeatsTolerated;
    [SerializeField] private int _repeatsChecked;

    private BurstRecorder _burstRecorder;
    private StreakRecorder _streakRecorder;

    private readonly HashSet<Vector3Int> _checkedRepeatPositions = new(); // For O(1) lookup of repeats
    private int _currentRepeats;

    private int _keysCorrect;
    private int _keysTyped;

    public event Action<StreakStat> OnStreakIncreased;
    public event Action OnStreakReset;

    public event Action<BurstStat> OnBurstMeasured;
    public event Action OnBurstReset;

    private void Awake()
    {
        _burstRecorder = GetComponent<BurstRecorder>();
        _streakRecorder = GetComponent<StreakRecorder>();
    }

    private void Start()
    {
        _streakRecorder.OnStreakIncreased += HandleStreakIncreased;
        _streakRecorder.OnStreakReset += HandleStreakReset;
        _burstRecorder.OnBurstMeasured += HandleBurstMeasured;
        _burstRecorder.OnBurstReset += HandleBurstReset;
    }

    private void OnDestroy()
    {
        if (_streakRecorder != null)
        {
            _streakRecorder.OnStreakIncreased -= HandleStreakIncreased;
            _streakRecorder.OnStreakReset -= HandleStreakReset;
        }
        if ( _burstRecorder != null)
        {
            _burstRecorder.OnBurstMeasured -= HandleBurstMeasured;
            _burstRecorder.OnBurstReset -= HandleBurstReset;
        }
    }

    public AccuracyStat CalculateAccuracy() => AccuracyStat.Calculate(_keysCorrect, _keysTyped);

    public StreakStat CalculateBestStreak() => _streakRecorder.CalculateBestStreak();

    public BurstStat CalculateTopSpeed() => _burstRecorder.CalculateTopSpeed();

    public void LogCorrectKey(KeyTile keyTile)
    {
        _keysCorrect++;
        _keysTyped++;

        if (TryValidateRepeats(keyTile))
        {
            IncreaseCheckedRepeats(keyTile);
            _streakRecorder.LogValidKey();
            _burstRecorder.LogValidKey(_keysCorrect, Time.time);
        }
        else
        {
            ResetRepeats();
            _streakRecorder.ResetStreak();
            _burstRecorder.ResetBursts();
        }
    }

    public void LogIncorrectKey()
    {
        _keysTyped++;

        _streakRecorder.LogIncorrectKey();
        _burstRecorder.LogIncorrectKey();
    }

    private bool TryValidateRepeats(KeyTile correctKeyTile)
    {
        var isRepeat = _checkedRepeatPositions.TryGetValue(correctKeyTile.Position, out var _);
        if (isRepeat)
            _currentRepeats++;

        return _currentRepeats < _repeatsTolerated;
    }

    private void IncreaseCheckedRepeats(KeyTile correctKeyTile)
    {
        _checkedRepeatPositions.Add(correctKeyTile.Position);
        if (_checkedRepeatPositions.Count > _repeatsChecked)
            _checkedRepeatPositions.Remove(_checkedRepeatPositions.First());
    }

    private void ResetRepeats()
    {
        _checkedRepeatPositions.Clear();
        _currentRepeats = 0;
    }

    private void HandleStreakIncreased(StreakStat streak) => OnStreakIncreased?.Invoke(streak);
    private void HandleStreakReset() => OnStreakReset?.Invoke();
    private void HandleBurstMeasured(BurstStat streak) => OnBurstMeasured?.Invoke(streak);
    private void HandleBurstReset() => OnBurstReset?.Invoke();
}

