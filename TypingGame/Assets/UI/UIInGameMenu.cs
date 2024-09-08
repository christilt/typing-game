using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInGameMenu : MonoBehaviour
{
    [SerializeField] private Hider _hider;
    [SerializeField] private UIInGameMenuSection _section1;
    [SerializeField] private UIInGameMenuSection _section2;
    [SerializeField] private float _moveInIntroDuration;
    [SerializeField] private float _moveInPositiveDuration;
    [SerializeField] private float _moveInNegativeDuration;
    [SerializeField] private float _moveOutDuration;
    [SerializeField] private float _text1Text2Delay;
    [SerializeField] private float _textFadeOutDuration;
    [SerializeField] private float _textFadeInDuration;

    private Tween _textSequence;

    public void MoveInIntroText(string text1, string text2, bool useOverlay = true, bool unscaledTime = true)
    {
        Func<UIInGameMenuSection, string, float, Tween> showTextAction = (uisection, text, duration) => uisection.MoveInIntroText(text, duration, unscaledTime);
        MoveInText(text1, text2, _moveInIntroDuration, showTextAction, useOverlay, unscaledTime);
    }

    public void MoveInPositiveText(string text1, string text2, bool useOverlay = true, bool unscaledTime = true)
    {
        Func<UIInGameMenuSection, string, float, Tween> showTextAction = (uisection, text, duration) => uisection.MoveInPositiveText(text, duration, unscaledTime);
        MoveInText(text1, text2, _moveInPositiveDuration, showTextAction, useOverlay, unscaledTime);
    }

    public void MoveInNegativeText(string text1, string text2, bool useOverlay = true, bool unscaledTime = true)
    {
        Func<UIInGameMenuSection, string, float, Tween> showTextAction = (uisection, text, duration) => uisection.MoveInNegativeText(text, duration, unscaledTime);
        MoveInText(text1, text2, _moveInNegativeDuration, showTextAction, useOverlay, unscaledTime);
    }

    public void MoveOutTextIfShown(bool unscaledTime = true)
    {
        _hider.Unhide(_moveOutDuration, unscaled: unscaledTime);

        _textSequence?.Kill();

        _section1.MoveOutTextIfShown(_moveOutDuration, unscaledTime);
        _section2.MoveOutTextIfShown(_moveOutDuration, unscaledTime);
    }

    public void FadeSwapText(string text1, string text2, bool unscaledTime = true)
    {
        _textSequence?.Kill();

        _textSequence = DOTween.Sequence()
            .Append(_section1.FadeOutText(_textFadeOutDuration, unscaledTime))
            .Join(_section2.FadeOutText(_textFadeOutDuration, unscaledTime))
            .Append(_section1.FadeInText(text1, _textFadeInDuration, unscaledTime))
            .Join(_section2.FadeInText(text2, _textFadeInDuration, unscaledTime))
            .SetUpdate(unscaledTime);
    }

    private void MoveInText(string text1, string text2, float duration, Func<UIInGameMenuSection, string, float, Tween> moveInTextAction, bool useOverlay = true, bool unscaledTime = true)
    {
        if (useOverlay)
            _hider.TransitionToOpaque(duration, unscaled: unscaledTime);


        Tween tween1 = default;
        if (!string.IsNullOrWhiteSpace(text1))
        {
            tween1 = moveInTextAction(_section1, text1, duration);
        }

        if (!string.IsNullOrWhiteSpace(text2))
        {
            var tween2 = moveInTextAction(_section2, text2, duration);

            if (tween1 != null)
            {
                _textSequence?.Kill();
                _textSequence = DOTween.Sequence()
                    .AppendInterval(_text1Text2Delay)
                    .Append(tween2)
                    .SetUpdate(unscaledTime);
            }
        }
    }
}