using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITextOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    [SerializeField] private Canvas _screenSpaceCanvas;
    [SerializeField] private Ease _textShowIntroEase;
    [SerializeField] private float _showIntroDuration;
    [SerializeField] private Ease _textShowPositiveEase;
    [SerializeField] private float _showPositiveDuration;
    [SerializeField] private Ease _textShowNegativeEase;
    [SerializeField] private float _showNegativeDuration;
    [SerializeField] private float _hideDuration;
    [SerializeField] private Ease _textHideEase;
    
    private Color _imageOriginalColor;

    private Vector3 _textStartPosition;
    private Vector3 _textShownPosition;
    private Vector3 _textHiddenPosition;

    private void Awake()
    {
        _screenSpaceCanvas = GetComponent<Canvas>();
        _imageOriginalColor = _image.color;

        _textShownPosition = _text.transform.position;
        _textStartPosition = _text.transform.position + new Vector3(0, _screenSpaceCanvas.pixelRect.yMax, 0);
        _textHiddenPosition = _text.transform.position - new Vector3(0, _screenSpaceCanvas.pixelRect.yMax, 0);
        _text.transform.position = _textStartPosition;
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

        if (useOverlay && _image.color.a != _imageOriginalColor.a)
            FadeInOverlay(unscaledTime);

        _text.transform.position = _textStartPosition;
        _text.text = text;

        _text.transform.DOMove(_textShownPosition, duration.Value)
            .SetEase(textEase.Value)
            .SetUpdate(unscaledTime);
    }

    public void HideTextIfShown(bool unscaledTime = true)
    {
        if (_image.color.a != 0)
            FadeOutOverlay(unscaledTime);

        if (_text.transform.position != _textHiddenPosition)
        {
            _text.transform.DOMove(_textHiddenPosition, _hideDuration)
                .SetEase(_textHideEase)
                .SetUpdate(unscaledTime);
        }
    }

    private void FadeInOverlay(bool unscaledTime) => _image.DOFade(_imageOriginalColor.a, _showIntroDuration).SetUpdate(unscaledTime);
    private void FadeOutOverlay(bool unscaledTime) => _image.DOFade(0, _hideDuration).SetUpdate(unscaledTime);
}