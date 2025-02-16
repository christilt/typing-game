using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PerformanceLogger : MonoBehaviour
{
    [SerializeField] private float _thresholdFps;
    [SerializeField] private float _logIntervalSeconds;

    private float _deltaTime;
    private float _fps;
    private float _logIntervalElapsedSeconds;
    private float _gcCountPrevious;
    private void Start()
    {
        _gcCountPrevious = GC.CollectionCount(0);
    }

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        _fps = 1f / _deltaTime;

        _logIntervalElapsedSeconds += Time.unscaledDeltaTime;
        if (_logIntervalElapsedSeconds < _logIntervalSeconds)
            return;
        _logIntervalElapsedSeconds = 0;

        var gcCount = GC.CollectionCount(0);
        var gcOccurred = gcCount != _gcCountPrevious;
        _gcCountPrevious = gcCount;
        
        if (_fps < _thresholdFps)
        {
            var message = new StringBuilder();
            message.Append($"{_fps:n01} FPS");

            if (gcOccurred)
            {
                message.Append("; ");
                message.Append($"GC occurred: {gcCount}");
            }

            Debug.LogWarning(message);
        }
    }
}