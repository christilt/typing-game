using DG.Tweening;
using System;
using UnityEngine;

public class Fader: MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    private Tween _tween;
    private float? _transitioningToAlpha;

    private void OnDestroy()
    {
        _tween?.Kill();
        _tween = null;
        _transitioningToAlpha = null;
    }

    public Tween Show(float duration, Ease ease, Action onComplete = null, bool unscaled = false)
    {
        return Transition(1, duration, ease, onComplete, unscaled);
    }

    public Tween Hide(float duration, Ease ease, Action onComplete = null, bool unscaled = false)
    {
        return Transition(0, duration, ease, onComplete, unscaled);
    }

    public Tween Transition(float alpha, float duration, Ease ease, Action onComplete = null, bool unscaled = false)
    {
        if (_transitioningToAlpha == alpha)
            return _tween;

        _transitioningToAlpha = alpha;
        var tween = _canvasGroup
            .DOFade(alpha, duration)
            .SetEase(ease)
            .SetUpdate(unscaled);

        tween.OnComplete(() =>
        {
            _transitioningToAlpha = null;
            if (onComplete != null)
                onComplete();
        });

        if (_tween != null)
        {
            _tween = DOTween.Sequence(_tween).Append(tween).SetUpdate(unscaled);
        }
        else
        {
            _tween = tween;
        }

        return _tween;
    }
}