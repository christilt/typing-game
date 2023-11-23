using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITextOverlay : MonoBehaviour
{
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

    private void Awake()
    {
        _screenSpaceCanvas = GetComponent<Canvas>();
    }

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

        // TODO
        if (useOverlay)
            LevelHider.Instance.Hider.FadeToStartAlpha(duration.Value, unscaled: unscaledTime);

        _text.transform.localPosition = _textStartPositionLocal;
        _text.text = text;

        _text.transform.DOLocalMove(_textShownPositionLocal, duration.Value)
            .SetEase(textEase.Value)
            .SetUpdate(unscaledTime);
    }

    public void HideTextIfShown(bool unscaledTime = true)
    {
        LevelHider.Instance.Hider.Unhide(_hideDuration, unscaled: unscaledTime);

        if (_text.transform.position != _textHiddenPositionLocal)
        {
            _text.transform.DOLocalMove(_textHiddenPositionLocal, _hideDuration)
                .SetEase(_textHideEase)
                .SetUpdate(unscaledTime);
        }
    }
}