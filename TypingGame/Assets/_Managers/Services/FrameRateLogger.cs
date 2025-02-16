using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FrameRateLogger : MonoBehaviour
{
    [SerializeField] private float _thresholdFps;
    [SerializeField] private float _logIntervalSeconds;

    private float _deltaTime;
    private float _fps;
    private float _logIntervalElapsedSeconds;


    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        _fps = 1f / _deltaTime;

        _logIntervalElapsedSeconds += Time.unscaledDeltaTime;
        if (_logIntervalElapsedSeconds < _logIntervalSeconds)
            return;
        _logIntervalElapsedSeconds = 0;
        
        if (_fps < _thresholdFps)
        {
            Debug.LogWarning($"{_fps:n01} FPS");
        }
    }
}