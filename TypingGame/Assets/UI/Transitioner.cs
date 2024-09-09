using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Transitioner : MonoBehaviour
{
    [SerializeField] private Hider _hider;
    [SerializeField] private MenuTransitionType _transitionType;

    private TransitionMover[] _movers;

    private Sequence _transitionSequence;

    private void Awake()
    {
        _movers = GetComponentsInChildren<TransitionMover>()
            .OrderByDescending(b => b.transform.position.y) // top to bottom
            .ToArray();

        _transitionSequence = DOTween.Sequence();
    }

    public Tween TransitionIn()
    {
        switch (_transitionType)
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
                throw new ArgumentOutOfRangeException(nameof(_transitionType), _transitionType, null);
        }
    }


    public Tween TransitionOut()
    {
        if (_transitionType.IsMove())
        {
            return MoveOut(
                GameSettingsManager.Instance.MenuTransitions.MoveOutDuration,
                GameSettingsManager.Instance.MenuTransitions.MoveOutEase);
        }
        else if (_transitionType.IsFade())
        {
            return FadeOut(
                GameSettingsManager.Instance.MenuTransitions.FadeOutDuration,
                GameSettingsManager.Instance.MenuTransitions.FadeOutEase);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(_transitionType), _transitionType, null);
        }
    }

    private Sequence MoveIn(float duration, Ease ease, bool unscaledTime = true)
    {
        if (_movers.Length == 0)
            return _transitionSequence;

        _transitionSequence?.Kill();
        _transitionSequence = DOTween.Sequence();

        for (var i = 0; i < _movers.Length; i++)
        {
            var mover = _movers[i];

            if (i > 0)
                _transitionSequence.AppendInterval(GameSettingsManager.Instance.MenuTransitions.IntervalDuration);

            var tween = mover.MoveIn(duration, ease, unscaledTime);
            _transitionSequence.Join(tween);
        }

        _transitionSequence.SetUpdate(unscaledTime);

        return _transitionSequence;
    }

    private Sequence MoveOut(float duration, Ease ease, bool unscaledTime = true)
    {
        if (_movers.Length == 0)
            return _transitionSequence;

        _transitionSequence?.Kill();
        _transitionSequence = DOTween.Sequence();

        for (var i = 0; i < _movers.Length; i++)
        {
            var mover = _movers[i];

            var tween = mover.MoveOutIfShown(duration, ease, unscaledTime);
            _transitionSequence.Join(tween);
        }

        _transitionSequence.SetUpdate(unscaledTime);

        return _transitionSequence;
    }

    private Sequence FadeIn(float duration, Ease ease, bool unscaledTime = true)
    {
        _transitionSequence?.Kill();

        var tween = _hider.Unhide(duration, unscaled: unscaledTime, ease: ease);
        _transitionSequence.Append(tween);
        _transitionSequence.SetUpdate(unscaledTime);

        return _transitionSequence;
    }

    private Sequence FadeOut(float duration, Ease ease, bool unscaledTime = true)
    {
        _transitionSequence?.Kill();

        var tween = _hider.HideCompletely(duration, unscaled: unscaledTime, ease: ease);
        _transitionSequence.Append(tween);
        _transitionSequence.SetUpdate(unscaledTime);

        return _transitionSequence;
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