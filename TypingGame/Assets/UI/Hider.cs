using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(SortingGroup))]
public class Hider : MonoBehaviour
{
    [SerializeField] private Image _hiderImage;
    [SerializeField] private Canvas _hiderCanvas;
    [SerializeField] private Camera _mainCamera; // Referencing Camera.main causes issues with scene loading additively
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
        _hiderCanvas.worldCamera = _mainCamera;
        _hiderCanvas.sortingLayerID = GetComponent<SortingGroup>().sortingLayerID;
        _opaqueAlphaFloat = _opaqueAlpha / 255f;
        _startAlphaFloat = _startAlpha / 255f;
        var startColour = _mainCamera.backgroundColor;
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

    public Tween Unhide(float duration, Action onComplete = null, bool unscaled = false, Ease? ease = null)
    {
        return Transition(0, duration, onComplete, unscaled, ease);
    }

    public Tween TransitionToOpaque(float duration, Action onComplete = null, bool unscaled = false)
    {
        return Transition(_opaqueAlphaFloat, duration, onComplete, unscaled);
    }

    public Tween HideCompletely(float duration, Action onComplete = null, bool unscaled = false, Ease? ease = null)
    {
        return Transition(1, duration, onComplete, unscaled, ease);
    }

    public Tween Transition(float alpha, float duration, Action onComplete = null, bool unscaled = false, Ease? ease = null)
    {
        if (_hiderImage.color.a == alpha)
            return _tween;

        if (_transitioningToAlpha == alpha)
            return _tween;

        _tween?.Kill();

        _transitioningToAlpha = alpha;

        ease ??= _hiderImage.color.a < alpha 
            ? _hideEase 
            : _unhideEase;
        _tween = _hiderImage
            .DOFade(alpha, duration)
            .SetEase(ease.Value)
            .SetUpdate(unscaled);

        _tween.OnComplete(() =>
        {
            _transitioningToAlpha = null;
            if (onComplete != null)
                onComplete();
        });

        return _tween;
    }
}