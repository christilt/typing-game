using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIInGameMenuSection : MonoBehaviour
{
    [SerializeField] private CanvasScaler _screenSpaceCanvasScaler;
    [SerializeField] private Ease _moveInIntroEase;
    [SerializeField] private Ease _moveInPositiveEase;
    [SerializeField] private Ease _moveInNegativeEase;
    [SerializeField] private Ease _moveOutEase;
    [SerializeField] private Ease _textFadeInEase;
    [SerializeField] private Ease _textFadeOutEase;

    [SerializeField] private TextMeshProUGUI _text;

    private Vector3 _startPositionLocal;
    private Vector3 _movedInPositionLocal;
    private Vector3 _movedOutPositionLocal;

    private Tween _tween;

    private Button _focusButton;

    private void Awake()
    {
        _focusButton = GetFirstButtonOrDefault();
    }

    private void Start()
    {
        _movedInPositionLocal = transform.localPosition;
        _startPositionLocal = transform.localPosition + new Vector3(0, _screenSpaceCanvasScaler.referenceResolution.y, 0);
        _movedOutPositionLocal = transform.localPosition - new Vector3(0, _screenSpaceCanvasScaler.referenceResolution.y, 0);
        transform.localPosition = _startPositionLocal;
    }

    public Tween MoveInIntroText(string text, float duration, bool unscaledTime = true)
    {
        return MoveInText(text, duration, _moveInIntroEase, unscaledTime);
    }

    public Tween MoveInPositiveText(string text, float duration, bool unscaledTime = true)
    {
        return MoveInText(text, duration, _moveInPositiveEase, unscaledTime);
    }

    public Tween MoveInNegativeText(string text, float duration, bool unscaledTime = true)
    {
        return MoveInText(text, duration, _moveInNegativeEase, unscaledTime);
    }

    private Tween MoveInText(string text, float duration, Ease? textEase = null, bool unscaledTime = true)
    {
        textEase ??= _moveInIntroEase;

        transform.localPosition = _startPositionLocal;
        SetText(text);

        _focusButton?.Select();

        _tween?.Kill();

        _tween = transform.DOLocalMove(_movedInPositionLocal, duration)
            .SetEase(textEase.Value)
            .SetUpdate(unscaledTime);
            

        return _tween;
    }

    public Tween MoveOutTextIfShown(float duration, bool unscaledTime = true)
    {
        if (IsTextShown())
        {
            _tween?.Kill();

            _tween = transform.DOLocalMove(_movedOutPositionLocal, duration)
                .SetEase(_moveOutEase)
                .SetUpdate(unscaledTime);
        }

        return _tween;
    }

    public Tween FadeOutTextIfShown(float duration, bool unscaledTime = true)
    {
        if (IsTextShown())
        {
            return FadeOutText(duration, unscaledTime);
        }

        return _tween;
    }

    public Tween FadeInText(string text, float duration, bool unscaledTime = true)
    {
        _tween?.Kill();

        _tween = DOTween.Sequence()
            .PrependCallback(() =>
            {
                transform.localPosition = _movedInPositionLocal;
                SetText(text);
            })
            .Append(_text.DOFade(1, duration))
            .SetEase(_textFadeInEase)
            .SetUpdate(unscaledTime);

        return _tween;
    }

    public Tween FadeOutText(float duration, bool unscaledTime = true)
    {
        _tween?.Kill();

        _tween = _text
            .DOFade(0, duration)
            .SetEase(_textFadeOutEase)
            .SetUpdate(unscaledTime);

        return _tween;
    }

    private void SetText(string text)
    {
        _text.text = TextHelper.WithPixelatedMonospaceText(text);
    }

    private bool IsTextShown() => transform.localPosition != _startPositionLocal && transform.localPosition != _movedOutPositionLocal && _text.material.color.a > 0;

    private Button GetFirstButtonOrDefault()
    {
        return GetComponentsInChildren<Button>()
            .Where(b => b.IsInteractable())
            .OrderByDescending(b => b.transform.position.y) // top to bottom
            .ThenBy(b => System.MathF.Abs(b.transform.position.x)) // middle to sides
            .FirstOrDefault();
    }
}