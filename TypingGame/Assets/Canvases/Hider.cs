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
    [SerializeField, Range(0,255)] private int _startAlpha;
    private float _startAlphaFloat;
    private Tween _tween;

    private void Awake()
    {
        _hiderCanvas.worldCamera = Camera.main;
        _hiderCanvas.sortingLayerID = GetComponent<SortingGroup>().sortingLayerID;
        _startAlphaFloat = _startAlpha / 255f;
        var startColour = Camera.main.backgroundColor;
        startColour.a = _startAlphaFloat;
        _hiderImage.color = startColour;
    }

    // TODO Show / Hide based on rate rather than duration?

    public void Unhide(float duration, Action onComplete = null, bool unscaled = false)
    {
        Fade(0, duration, onComplete, unscaled);
    }

    public void FadeToStartAlpha(float duration, Action onComplete = null, bool unscaled = false)
    {
        Fade(_startAlphaFloat, duration, onComplete, unscaled);
    }

    public void HideCompletely(float duration, Action onComplete = null, bool unscaled = false)
    {
        Fade(1, duration, onComplete, unscaled);
    }

    public void Fade(float alpha, float duration, Action onComplete = null, bool unscaled = false)
    {
        if (_hiderImage.color.a == alpha)
            return;

        if (_tween != null)
            _tween.Kill();

        _tween = _hiderImage
            .DOFade(alpha, duration)
            .SetUpdate(unscaled);

        if (onComplete != null)
            _tween.OnComplete(() => onComplete());
    }
}