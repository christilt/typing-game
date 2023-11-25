using System;
using UnityEngine;

[RequireComponent (typeof(Hider))]
public class SceneHider : Singleton<SceneHider>
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