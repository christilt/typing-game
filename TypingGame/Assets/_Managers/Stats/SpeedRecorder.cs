using System;
using UnityEngine;

public class SpeedRecorder : MonoBehaviour
{
    private bool _isRecording;

    private void Start()
    {
        _isRecording = true;
    }

    public float TimeTakenSeconds { get; private set; }

    public void Stop()
    {
        _isRecording = false;
    }

    public SpeedStat CalculateSpeed(float benchmarkDurationSeconds)
    {
        return SpeedStat.Calculate(
            timeTaken: TimeSpan.FromSeconds(TimeTakenSeconds), 
            timeBenchmark: TimeSpan.FromSeconds(benchmarkDurationSeconds));
    }

    private void Update()
    {
        if (!_isRecording)
            return;

        TimeTakenSeconds += Time.deltaTime;
    }
}
