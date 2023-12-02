using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITextOverlay : MonoBehaviour
{
    [SerializeField] private Hider _hider;
    [SerializeField] private UIText _text1;
    [SerializeField] private UIText _text2;
    [SerializeField] private float _showIntroDuration;
    [SerializeField] private float _showPositiveDuration;
    [SerializeField] private float _showNegativeDuration;
    [SerializeField] private float _hideDuration;
    [SerializeField] private float _text1Text2Delay;

    private Sequence _delayThenText2Sequence;

    public void ShowIntroText(string text1, string text2, bool useOverlay = true, bool unscaledTime = true)
    {
        Func<UIText, string, float, Tween> showTextAction = (uitext, text, duration) => uitext.ShowIntroText(text, duration, unscaledTime);
        ShowText(text1, text2, _showIntroDuration, showTextAction, useOverlay, unscaledTime);
    }

    public void ShowPositiveText(string text1, string text2, bool useOverlay = true, bool unscaledTime = true)
    {
        Func<UIText, string, float, Tween> showTextAction = (uitext, text, duration) => uitext.ShowPositiveText(text, duration, unscaledTime);
        ShowText(text1, text2, _showPositiveDuration, showTextAction, useOverlay, unscaledTime);
    }

    public void ShowNegativeText(string text1, string text2, bool useOverlay = true, bool unscaledTime = true)
    {
        Func<UIText, string, float, Tween> showTextAction = (uitext, text, duration) => uitext.ShowNegativeText(text, duration, unscaledTime);
        ShowText(text1, text2, _showNegativeDuration, showTextAction, useOverlay, unscaledTime);
    }

    private void ShowText(string text1, string text2, float duration, Func<UIText, string, float, Tween> showTextAction, bool useOverlay = true, bool unscaledTime = true)
    {
        if (useOverlay)
            _hider.TransitionToOpaque(duration, unscaled: unscaledTime);


        Tween tween1 = default;
        if (!string.IsNullOrWhiteSpace(text1))
        {
            tween1 = showTextAction(_text1, text1, duration);
        }

        if (!string.IsNullOrWhiteSpace(text2))
        {
            var tween2 = showTextAction(_text2, text2, duration);

            if (tween1 != null)
            {
                _delayThenText2Sequence?.Kill();
                _delayThenText2Sequence = DOTween.Sequence()
                    .AppendInterval(_text1Text2Delay)
                    .Append(tween2)
                    .SetUpdate(unscaledTime);
            }
        }
    }

    public void HideTextIfShown(bool unscaledTime = true)
    {
        _hider.Unhide(_hideDuration, unscaled: unscaledTime);

        _delayThenText2Sequence?.Kill();

        _text1.HideTextIfShown(_hideDuration, unscaledTime);
        _text2.HideTextIfShown(_hideDuration, unscaledTime);
    }
}