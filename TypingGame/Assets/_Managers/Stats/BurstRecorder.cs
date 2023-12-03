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

    private readonly List<BurstMeasurement> _currentBurstMeasurements = new();
    private Queue<MeasureBurstsRequest> _burstMeasurementRequests = new();

    private void Start()
    {
        StartCoroutine(ProcessBurstMeasurement());
    }

    public void MeasureBursts(int keyNumber, float timeKeyLogged, Action<Burst> onBurstMeasured = null)
    {
        onBurstMeasured ??= (Burst _) => { };

        _burstMeasurementRequests.Enqueue(new MeasureBurstsRequest(keyNumber, timeKeyLogged, onBurstMeasured));
    }

    public void ResetBursts()
    {
        StopAllCoroutines();

        _currentBurstMeasurements.Clear();
        _burstMeasurementRequests.Clear();

        StartCoroutine(ProcessBurstMeasurement());
    }

    // TODO: Is a coroutine enough for performance here?
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
                    var keysPerSecond = keys / duration;
                    var wordsPerMinute = ToWordsPerMinute(keysPerSecond);
                    var burst = new Burst(keys, duration, keysPerSecond, wordsPerMinute);

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

    private float ToWordsPerMinute(float keysPerSecond)
    {
        // Consensus whole number based on various sources, and based on usage rather than dictionary
        // e.g. https://www.ilovelanguages.com/how-many-letters-does-the-average-english-word-have/, https://en.ans.wiki/6074/what-is-the-average-number-of-letters-per-word/
        const int AverageLettersPerWord = 5;

        var lettersPerMinute = keysPerSecond * 60;
        var wordsPerMinute = lettersPerMinute / AverageLettersPerWord;
        return wordsPerMinute;
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
        public Dictionary<int, Burst> BurstsByKeyNumberTo { get; }
    }

    private class MeasureBurstsRequest
    {
        public MeasureBurstsRequest(int keyNumber, float keyTimeLogged, Action<Burst> onBurstMeasured)
        {
            KeyNumber = keyNumber;
            KeyTimeLogged = keyTimeLogged;
            OnBurstMeasured = onBurstMeasured;
            IsProcessing = false;
        }

        public int KeyNumber { get; }
        public float KeyTimeLogged { get; }
        public Action<Burst> OnBurstMeasured { get; }
        public bool IsProcessing { get; set; }
    }
}

public struct Burst
{
    public Burst(int keys, float duration, float keysPerSecond, float wordsPerMinute)
    {
        Keys = keys;
        Duration = duration;
        KeysPerSecond = keysPerSecond;
        WordsPerMinute = wordsPerMinute;
    }

    public int Keys { get; }
    public float Duration { get; }
    public float KeysPerSecond { get; }
    public float WordsPerMinute { get; }

    public bool IsBetterThan(Burst other)
    {
        // TODO: Does this need to be more clever?

        return KeysPerSecond > other.KeysPerSecond;
    }

    public override string ToString()
    {
        return $"{WordsPerMinute:n1} wpm ({Keys}keys / {Duration:n0}s)";
    }
}