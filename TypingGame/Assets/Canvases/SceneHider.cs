using System;
using UnityEngine;

[RequireComponent (typeof(Hider))]
public class SceneHider : PersistentSingleton<SceneHider>
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
        MakeOpaque(_startLevelFadeDuration, onComplete, unscaled: true);
    }

    public void MakeOpaque(float duration, Action onComplete = null, bool unscaled = true)
    {
        _hider.TransitionToOpaque(duration, onComplete, unscaled);
    }

    public void EndOfSceneFadeOut(Action onComplete = null)
    {
        _hider.HideCompletely(_endLevelFadeDuration, onComplete, unscaled: true);
    }

    public void Unhide(float duration, Action onComplete = null)
    {
        _hider.Unhide(duration, onComplete, unscaled: true);
    }
}