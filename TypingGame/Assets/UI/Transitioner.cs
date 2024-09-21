using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Transitioner : MonoBehaviour
{
    [SerializeField] private Fader _fader;
    [SerializeField] private MenuTransitionType _transitionInType;
    [SerializeField] private MenuTransitionType _transitionOutType;

    private TransitionMover[] _movers;

    private Tween _transitionTween;

    private void Awake()
    {
        _movers = GetComponentsInChildren<TransitionMover>()
            .Where(o => o.gameObject.transform.parent == transform) // immediate children only
            .OrderByDescending(b => b.transform.position.y) // top to bottom
            .ToArray();

        _transitionTween = DOTween.Sequence();
    }

    public Tween TransitionIn(MenuTransitionType? type = null)
    {
        type ??= _transitionInType;

        switch (type)
        {
            case MenuTransitionType.Move_Intro:
                return MoveIn(
                    GameSettingsManager.Instance.MenuTransitions.MoveInIntroDuration,
                    GameSettingsManager.Instance.MenuTransitions.MoveInIntroEase);
            case MenuTransitionType.Move_Positive:
                return MoveIn(
                    GameSettingsManager.Instance.MenuTransitions.MoveInPositiveDuration,
                    GameSettingsManager.Instance.MenuTransitions.MoveInPositiveEase);
            case MenuTransitionType.Move_Negative:
                return MoveIn(
                    GameSettingsManager.Instance.MenuTransitions.MoveInNegativeDuration,
                    GameSettingsManager.Instance.MenuTransitions.MoveInNegativeEase);
            case MenuTransitionType.Fade:
                return FadeIn(
                    GameSettingsManager.Instance.MenuTransitions.FadeInDuration,
                    GameSettingsManager.Instance.MenuTransitions.FadeInEase);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }


    public Tween TransitionOut(MenuTransitionType? type = null)
    {
        type ??= _transitionOutType;

        if (type.Value.IsMove())
        {
            return MoveOut(
                GameSettingsManager.Instance.MenuTransitions.MoveOutDuration,
                GameSettingsManager.Instance.MenuTransitions.MoveOutEase);
        }
        else if (type.Value.IsFade())
        {
            return FadeOut(
                GameSettingsManager.Instance.MenuTransitions.FadeOutDuration,
                GameSettingsManager.Instance.MenuTransitions.FadeOutEase);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(type), type.Value, null);
        }
    }

    public Tween TransitionInRepeated() => TransitionIn(MenuTransitionType.Fade);

    private Tween MoveIn(float duration, Ease ease, bool unscaledTime = true)
    {
        if (_movers.Length == 0)
            return _transitionTween;

        _transitionTween?.Kill();
        var sequence = DOTween.Sequence();

        for (var i = 0; i < _movers.Length; i++)
        {
            var mover = _movers[i];

            if (i > 0)
                sequence.AppendInterval(GameSettingsManager.Instance.MenuTransitions.IntervalDuration);

            var tween = mover.MoveIn(duration, ease, unscaledTime);
            sequence.Join(tween);
        }

        sequence.SetUpdate(unscaledTime);

        _transitionTween = sequence;

        return _transitionTween;
    }

    private Tween MoveOut(float duration, Ease ease, bool unscaledTime = true)
    {
        if (_movers.Length == 0)
            return _transitionTween;

        _transitionTween?.Kill();
        var sequence = DOTween.Sequence();

        for (var i = 0; i < _movers.Length; i++)
        {
            var mover = _movers[i];

            var tween = mover.MoveOutIfShown(duration, ease, unscaledTime);
            sequence.Join(tween);
        }

        sequence.SetUpdate(unscaledTime);

        _transitionTween = sequence;

        return _transitionTween;
    }

    private Tween FadeIn(float duration, Ease ease, bool unscaledTime = true)
    {
        _transitionTween?.Kill();
        _transitionTween = _fader
            .Show(duration, unscaled: unscaledTime, ease: ease)
            .SetUpdate(unscaledTime); 

        return _transitionTween;
    }

    private Tween FadeOut(float duration, Ease ease, bool unscaledTime = true)
    {
        _transitionTween?.Kill();
        _transitionTween = _fader
            .Hide(duration, unscaled: unscaledTime, ease: ease)
            .SetUpdate(unscaledTime);

        return _transitionTween;
    }
}

public enum MenuTransitionType
{
    Move_Intro,
    Move_Positive,
    Move_Negative,
    Fade
}

public static class MenuTransitionTypeExtensions
{
    public static bool IsMove(this MenuTransitionType type)
    {
        return type == MenuTransitionType.Move_Intro
            || type == MenuTransitionType.Move_Positive
            || type == MenuTransitionType.Move_Negative;
    }

    public static bool IsFade(this MenuTransitionType type)
    {
        return type == MenuTransitionType.Fade;
    }
}