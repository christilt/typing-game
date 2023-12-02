using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIText : MonoBehaviour
{
    [SerializeField] private Canvas _screenSpaceCanvas;
    [SerializeField] private Ease _textShowIntroEase;
    [SerializeField] private Ease _textShowPositiveEase;
    [SerializeField] private Ease _textShowNegativeEase;
    [SerializeField] private Ease _textHideEase;

    private TextMeshProUGUI _text;
    private Vector3 _textStartPositionLocal;
    private Vector3 _textShownPositionLocal;
    private Vector3 _textHiddenPositionLocal;

    private Tween _tween;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _textShownPositionLocal = _text.transform.localPosition;
        _textStartPositionLocal = _text.transform.localPosition + new Vector3(0, _screenSpaceCanvas.pixelRect.yMax, 0);
        _textHiddenPositionLocal = _text.transform.localPosition - new Vector3(0, _screenSpaceCanvas.pixelRect.yMax, 0);
        _text.transform.localPosition = _textStartPositionLocal;
    }

    public Tween ShowIntroText(string text, float duration, bool unscaledTime = true)
    {
        return ShowText(text, duration, _textShowIntroEase, unscaledTime);
    }

    public Tween ShowPositiveText(string text, float duration, bool unscaledTime = true)
    {
        return ShowText(text, duration, _textShowPositiveEase, unscaledTime);
    }

    public Tween ShowNegativeText(string text, float duration, bool unscaledTime = true)
    {
        return ShowText(text, duration, _textShowNegativeEase, unscaledTime);
    }

    private Tween ShowText(string text, float duration, Ease? textEase = null, bool unscaledTime = true)
    {
        textEase ??= _textShowIntroEase;

        _text.transform.localPosition = _textStartPositionLocal;
        _text.text = TextHelper.WithPixelatedMonospaceText(text);

        _tween?.Kill();

        _tween = _text.transform.DOLocalMove(_textShownPositionLocal, duration)
            .SetEase(textEase.Value)
            .SetUpdate(unscaledTime);

        return _tween;
    }

    public Tween HideTextIfShown(float duration, bool unscaledTime = true)
    {
        if (IsTextShown())
        {
            _tween?.Kill();

            _tween = _text.transform.DOLocalMove(_textHiddenPositionLocal, duration)
                .SetEase(_textHideEase)
                .SetUpdate(unscaledTime);
        }

        return _tween;
    }

    private bool IsTextShown() => _text.transform.localPosition != _textStartPositionLocal && _text.transform.localPosition != _textHiddenPositionLocal;
}