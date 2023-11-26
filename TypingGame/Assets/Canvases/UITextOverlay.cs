using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO use own overlay and not scene hider - scene hider should hide everything
public class UITextOverlay : MonoBehaviour
{
    [SerializeField] private Hider _hider;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Canvas _screenSpaceCanvas;
    [SerializeField] private Ease _textShowIntroEase;
    [SerializeField] private float _showIntroDuration;
    [SerializeField] private Ease _textShowPositiveEase;
    [SerializeField] private float _showPositiveDuration;
    [SerializeField] private Ease _textShowNegativeEase;
    [SerializeField] private float _showNegativeDuration;
    [SerializeField] private float _hideDuration;
    [SerializeField] private Ease _textHideEase;

    private Vector3 _textStartPositionLocal;
    private Vector3 _textShownPositionLocal;
    private Vector3 _textHiddenPositionLocal;

    private Tween _tween;

    private void Start()
    {
        _textShownPositionLocal = _text.transform.localPosition;
        _textStartPositionLocal = _text.transform.localPosition + new Vector3(0, _screenSpaceCanvas.pixelRect.yMax, 0);
        _textHiddenPositionLocal = _text.transform.localPosition - new Vector3(0, _screenSpaceCanvas.pixelRect.yMax, 0);
        _text.transform.localPosition = _textStartPositionLocal;
    }

    public void ShowIntroText(string text, bool useOverlay = true, bool unscaledTime = true)
    {
        ShowText(text, _textShowIntroEase, _showIntroDuration, useOverlay, unscaledTime);
    }

    public void ShowPositiveText(string text, bool useOverlay = true, bool unscaledTime = true)
    {
        ShowText(text, _textShowPositiveEase, _showPositiveDuration, useOverlay, unscaledTime);
    }

    public void ShowNegativeText(string text, bool useOverlay = true, bool unscaledTime = true)
    {
        ShowText(text, _textShowNegativeEase, _showNegativeDuration, useOverlay, unscaledTime);
    }

    private void ShowText(string text, Ease? textEase = null, float? duration = null, bool useOverlay = true, bool unscaledTime = true)
    {
        textEase ??= _textShowIntroEase;
        duration ??= _showIntroDuration;

        if (useOverlay)
            _hider.TransitionToOpaque(duration.Value, unscaled: unscaledTime);

        _text.transform.localPosition = _textStartPositionLocal;
        _text.text = TextHelper.WithPixelatedMonospaceText(text);

        _tween?.Kill();

        _tween = _text.transform.DOLocalMove(_textShownPositionLocal, duration.Value)
            .SetEase(textEase.Value)
            .SetUpdate(unscaledTime);
    }

    public void HideTextIfShown(bool unscaledTime = true)
    {
        _hider.Unhide(_hideDuration);

        if (IsTextShown())
        {
            _tween?.Kill();

            _tween = _text.transform.DOLocalMove(_textHiddenPositionLocal, _hideDuration)
                .SetEase(_textHideEase)
                .SetUpdate(unscaledTime);
        }
    }

    private bool IsTextShown() => _text.transform.localPosition != _textStartPositionLocal && _text.transform.localPosition != _textHiddenPositionLocal;
}