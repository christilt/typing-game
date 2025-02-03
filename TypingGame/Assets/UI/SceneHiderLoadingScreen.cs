using System;
using UnityEngine;

/// <summary>
/// Another class with same functionality as SceneHider.  Ensures no confusion between SceneHider instances when loading scenes additively
/// </summary>
[RequireComponent(typeof(Hider))]
public class SceneHiderLoadingScreen : Singleton<SceneHiderLoadingScreen>
{
    private Hider _hider;
    [SerializeField] private float _startLevelFadeDuration;
    [SerializeField] private float _endLevelFadeDuration;

    protected override void Awake()
    {
        base.Awake();

        _hider = GetComponent<Hider>();
    }

    public void StartOfSceneFadeIn(Action onComplete = null)
    {
        _hider.Unhide(_startLevelFadeDuration, onComplete, unscaled: true);
    }

    public void EndOfSceneFadeOut(Action onComplete = null)
    {
        _hider.HideCompletely(_endLevelFadeDuration, onComplete, unscaled: true);
    }
}