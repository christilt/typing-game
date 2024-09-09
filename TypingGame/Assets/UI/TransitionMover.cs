using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TransitionMover : MonoBehaviour
{
    [SerializeField] private CanvasScaler _screenSpaceCanvasScaler;

    private Vector3 _hiddenAbovePositionLocal;
    private Vector3 _shownPositionLocal;
    private Vector3 _hiddenBelowPositionLocal;

    private Tween _transitionInTween;

    private void Awake()
    {
        _shownPositionLocal = transform.localPosition;
        _hiddenAbovePositionLocal = transform.localPosition + new Vector3(0, _screenSpaceCanvasScaler.referenceResolution.y, 0);
        _hiddenBelowPositionLocal = transform.localPosition - new Vector3(0, _screenSpaceCanvasScaler.referenceResolution.y, 0);
    }

    public Tween MoveIn(float duration, Ease ease, bool unscaledTime = true)
    {
        transform.localPosition = _hiddenAbovePositionLocal;

        _transitionInTween?.Kill();

        _transitionInTween = transform.DOLocalMove(_shownPositionLocal, duration)
            .SetEase(ease)
            .SetUpdate(unscaledTime);

        return _transitionInTween;
    }

    public Tween MoveOutIfShown(float duration, Ease ease, bool unscaledTime = true)
    {
        if (IsShown())
        {
            _transitionInTween?.Kill();

            _transitionInTween = transform.DOLocalMove(_hiddenBelowPositionLocal, duration)
                .SetEase(ease)
                .SetUpdate(unscaledTime);
        }
        else
        {
            _transitionInTween = DOTween.Sequence();
        }

        return _transitionInTween;
    }

    private bool IsShown() => transform.localPosition != _hiddenAbovePositionLocal && transform.localPosition != _hiddenBelowPositionLocal;
}