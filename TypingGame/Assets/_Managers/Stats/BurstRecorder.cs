using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// TODO: Maybe record short, medium and long bursts instead of measuring one best over a range - best is always the shortest number of keys
public class BurstRecorder : MonoBehaviour
{
    [SerializeField] private int _burstMinimumChecked;
    [SerializeField] private int _burstMaximumChecked;
    [SerializeField] private float _burstProcessingInterval;
    [SerializeField] private int _burstMistakesTolerated;

    [SerializeField] private StatCategory[] _notifyForCategories;
    [SerializeField] private float _notifyIntervalSeconds;

    private readonly Queue<MeasureBurstsRequest> _burstMeasurementRequests = new();
    private readonly List<BurstMeasurement> _currentBurstMeasurements = new();

    private BurstStat _bestBurst;
    private int _currentBurstMistakes;
    private float _notifyIntervalSecondsRemaining;

    private void Start()
    {
        StartCoroutine(ProcessBurstMeasurement());
    }

    private void Update()
    {
        _notifyIntervalSecondsRemaining -= Time.deltaTime;
    }

    public event Action<BurstStat> OnBurstMeasured;
    public event Action<BurstStat> OnBurstNotification;
    public event Action OnBurstReset;

    public BurstStat CalculateTopSpeed() => _bestBurst;

    public void LogValidKey(int keyNumber, float timeKeyLogged)
    {
        _burstMeasurementRequests.Enqueue(new MeasureBurstsRequest(keyNumber, timeKeyLogged, measuredBurst =>
        {
            OnBurstMeasured?.Invoke(measuredBurst);
            MaybeNotify(measuredBurst);
            if (measuredBurst.IsBetterThan(_bestBurst))
            {
                _bestBurst = measuredBurst;

                //Debug.Log($"New best burst: {measuredBurst}");
            }
        }));
    }

    public void LogIncorrectKey()
    {
        _currentBurstMistakes++;
        if (_currentBurstMistakes > _burstMistakesTolerated)
            ResetBursts();
    }

    public void ResetBursts()
    {
        StopAllCoroutines();

        _currentBurstMeasurements.Clear();
        _burstMeasurementRequests.Clear();
        _currentBurstMistakes = 0;
        OnBurstReset?.Invoke();

        StartCoroutine(ProcessBurstMeasurement());
    }

    // TODO: Consider simplifying if a performance concern - logging after every e.g. 5 keys might be enough
    private IEnumerator ProcessBurstMeasurement()
    {
        while (true)
        {
            yield return new WaitForSeconds(_burstProcessingInterval);

            if (_burstMeasurementRequests.Count == 0)
                continue;

            var request = _burstMeasurementRequests.Peek();
            if (request.IsProcessing)
                continue;

            request.IsProcessing = true;

            _currentBurstMeasurements.Add(new BurstMeasurement(request.KeyNumber, request.KeyTimeLogged));
            if (_currentBurstMeasurements.Count > _burstMaximumChecked)
                _currentBurstMeasurements.RemoveAt(0);

            for (var indexFrom = 0; indexFrom < _currentBurstMeasurements.Count - _burstMinimumChecked; indexFrom++)
            {
                var burstMeasurementFrom = _currentBurstMeasurements[indexFrom];
                for (var indexTo = indexFrom + _burstMinimumChecked; indexTo < _currentBurstMeasurements.Count; indexTo++)
                {
                    var burstMeasurementTo = _currentBurstMeasurements[indexTo];
                    if (burstMeasurementFrom.BurstsByKeyNumberTo.ContainsKey(burstMeasurementTo.KeyNumber))
                        continue;

                    var keys = burstMeasurementTo.KeyNumber - burstMeasurementFrom.KeyNumber;
                    var duration = burstMeasurementTo.KeyTimeLogged - burstMeasurementFrom.KeyTimeLogged;
                    var burst = BurstStat.Calculate(keys, duration);

                    burstMeasurementFrom.BurstsByKeyNumberTo.Add(burstMeasurementTo.KeyNumber, burst);

                    request.OnBurstMeasured(burst);

                    yield return new WaitForEndOfFrame();
                }

                //Debug.Log(
                //    $"Burst measurement for key {burstMeasurementFrom.KeyNumber} now has {burstMeasurementFrom.BurstsByKeyNumberTo.Count} bursts:{Environment.NewLine}" +
                //    $"{string.Join(Environment.NewLine, burstMeasurementFrom.BurstsByKeyNumberTo.Select(b => $"{b.Key}: {b.Value}"))}");
            }

            //Debug.Log($"Dequeuing request for {request.KeyNumber}");
            _burstMeasurementRequests.Dequeue();
        }
    }

    private void MaybeNotify(BurstStat burst)
    {
        if (_notifyIntervalSecondsRemaining > 0)
            return;

        if (!_notifyForCategories.Contains(burst.Category))
            return;

        OnBurstNotification?.Invoke(burst);

        _notifyIntervalSecondsRemaining = _notifyIntervalSeconds;
    }

    private class BurstMeasurement
    {
        public BurstMeasurement(int keyNumber, float keyTimeLogged)
        {
            KeyNumber = keyNumber;
            KeyTimeLogged = keyTimeLogged;
            BurstsByKeyNumberTo = new();
        }

        public int KeyNumber { get; }
        public float KeyTimeLogged { get; }
        public Dictionary<int, BurstStat> BurstsByKeyNumberTo { get; }
    }

    private class MeasureBurstsRequest
    {
        public MeasureBurstsRequest(int keyNumber, float keyTimeLogged, Action<BurstStat> onBurstMeasured)
        {
            KeyNumber = keyNumber;
            KeyTimeLogged = keyTimeLogged;
            OnBurstMeasured = onBurstMeasured;
            IsProcessing = false;
        }

        public int KeyNumber { get; }
        public float KeyTimeLogged { get; }
        public Action<BurstStat> OnBurstMeasured { get; }
        public bool IsProcessing { get; set; }
    }
}
