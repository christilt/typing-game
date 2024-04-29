using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIText : MonoBehaviour
{
    [SerializeField] private CanvasScaler _screenSpaceCanvasScaler;
    [SerializeField] private Ease _textMoveInIntroEase;
    [SerializeField] private Ease _textMoveInPositiveEase;
    [SerializeField] private Ease _textMoveInNegativeEase;
    [SerializeField] private Ease _textMoveOutEase;
    [SerializeField] private Ease _textFadeInEase;
    [SerializeField] private Ease _textFadeOutEase;

    private TextMeshProUGUI _text;
    private Vector3 _textStartPositionLocal;
    private Vector3 _textMovedInPositionLocal;
    private Vector3 _textMovedOutPositionLocal;

    private Tween _tween;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _textMovedInPositionLocal = _text.transform.localPosition;
        _textStartPositionLocal = _text.transform.localPosition + new Vector3(0, _screenSpaceCanvasScaler.referenceResolution.y, 0);
        _textMovedOutPositionLocal = _text.transform.localPosition - new Vector3(0, _screenSpaceCanvasScaler.referenceResolution.y, 0);
        _text.transform.localPosition = _textStartPositionLocal;
    }

    public Tween MoveInIntroText(string text, float duration, bool unscaledTime = true)
    {
        return MoveInText(text, duration, _textMoveInIntroEase, unscaledTime);
    }

    public Tween MoveInPositiveText(string text, float duration, bool unscaledTime = true)
    {
        return MoveInText(text, duration, _textMoveInPositiveEase, unscaledTime);
    }

    public Tween MoveInNegativeText(string text, float duration, bool unscaledTime = true)
    {
        return MoveInText(text, duration, _textMoveInNegativeEase, unscaledTime);
    }

    private Tween MoveInText(string text, float duration, Ease? textEase = null, bool unscaledTime = true)
    {
        textEase ??= _textMoveInIntroEase;

        _text.transform.localPosition = _textStartPositionLocal;
        SetText(text);

        _tween?.Kill();

        _tween = _text.transform.DOLocalMove(_textMovedInPositionLocal, duration)
            .SetEase(textEase.Value)
            .SetUpdate(unscaledTime);

        return _tween;
    }

    public Tween MoveOutTextIfShown(float duration, bool unscaledTime = true)
    {
        if (IsTextShown())
        {
            _tween?.Kill();

            _tween = _text.transform.DOLocalMove(_textMovedOutPositionLocal, duration)
                .SetEase(_textMoveOutEase)
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
                _text.transform.localPosition = _textMovedInPositionLocal;
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

    private bool IsTextShown() => _text.transform.localPosition != _textStartPositionLocal && _text.transform.localPosition != _textMovedOutPositionLocal && _text.material.color.a > 0;
}