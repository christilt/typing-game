﻿using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(SortingGroup))]
public class Hider : MonoBehaviour
{
    [SerializeField] private Image _hiderImage;
    [SerializeField] private Canvas _hiderCanvas;
    [SerializeField, Range(0, 255)] private int _startAlpha;
    [SerializeField, Range(0, 255)] private int _opaqueAlpha;
    [SerializeField] private Ease _hideEase;
    [SerializeField] private Ease _unhideEase;
    private float _startAlphaFloat;
    private float _opaqueAlphaFloat;
    private Tween _tween;
    private float? _transitioningToAlpha;

    private void Awake()
    {
        _hiderCanvas.worldCamera = Camera.main;
        _hiderCanvas.sortingLayerID = GetComponent<SortingGroup>().sortingLayerID;
        _opaqueAlphaFloat = _opaqueAlpha / 255f;
        _startAlphaFloat = _startAlpha / 255f;
        var startColour = Camera.main.backgroundColor;
        startColour.a = _startAlphaFloat;
        _hiderImage.color = startColour;
    }

    private void OnDestroy()
    {
        _tween?.Kill();
        _tween = null;
        _transitioningToAlpha = null;
    }

    // TODO Show / Hide based on rate rather than duration?

    public void Unhide(float duration, Action onComplete = null, bool unscaled = false)
    {
        Transition(0, duration, onComplete, unscaled);
    }

    public void TransitionToOpaque(float duration, Action onComplete = null, bool unscaled = false)
    {
        Transition(_opaqueAlphaFloat, duration, onComplete, unscaled);
    }

    public void HideCompletely(float duration, Action onComplete = null, bool unscaled = false)
    {
        Transition(1, duration, onComplete, unscaled);
    }

    public void Transition(float alpha, float duration, Action onComplete = null, bool unscaled = false)
    {
        if (_hiderImage.color.a == alpha)
            return;

        if (_transitioningToAlpha == alpha)
            return;

        _tween?.Kill();

        _transitioningToAlpha = alpha;

        var isHide = _hiderImage.color.a < alpha;
        var ease = isHide ? _hideEase : _unhideEase;
        _tween = _hiderImage
            .DOFade(alpha, duration)
            .SetEase(ease)
            .SetUpdate(unscaled);

        _tween.OnComplete(() =>
        {
            _transitioningToAlpha = null;
            if (onComplete != null)
                onComplete();
        });

    }
}