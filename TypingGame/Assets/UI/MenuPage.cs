﻿using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuPage : MonoBehaviour
{
    [SerializeField] private MenuPage _nextPage;
    private MenuPage _previousPage;
    private Button _firstButton;
    private Animator[] _animators;
    private int _enableCount;

    private Transitioner _optionalTransitioner;

    private void Awake()
    {
        _optionalTransitioner = GetComponent<Transitioner>();
        _animators = GetComponentsInChildren<Animator>();
    }

    private void OnEnable()
    {
        _enableCount++;

        _firstButton ??= GetFirstButtonOrDefault();
        if (_firstButton == null)
            return;

        if (_optionalTransitioner != null)
        {
            var tween = _enableCount == 1
                ? _optionalTransitioner.TransitionIn()
                : _optionalTransitioner.TransitionInRepeated();

            tween.OnComplete(() => _firstButton.Select());
        }
        else
        {
            _firstButton.Select();
        }
    }

    public void OpenNext(MenuPage page)
    {
        if (page == null)
        {
            Debug.LogError("Page to open is null");
            return;
        }

        _nextPage = page;

        _nextPage.OpenFromPrevious(this);
    }

    public void OpenNext() => OpenNext(_nextPage);

    public void OpenPrevious()
    {
        if (_previousPage == null)
        {
            Debug.LogError("Page to open is null");
            return;
        }

        _previousPage.OpenFromNext();
    }

    public Tween Disable()
    {
        if (_optionalTransitioner != null)
        {
            return _optionalTransitioner
                .TransitionOut()
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
        else
        {
            gameObject.SetActive(false);

            return DOTween.Sequence();
        }
    }

    private void OpenFromPrevious(MenuPage previous)
    {
        if (_previousPage == null)
            _previousPage = previous;

        DOTween.Sequence()
            .Append(_previousPage.Disable())
            .OnComplete(() => gameObject.SetActive(true))
            .SetUpdate(true);
    }

    private void OpenFromNext()
    {
        DOTween.Sequence()
            .Append(_nextPage.Disable())
            .OnComplete(() => gameObject.SetActive(true))
            .SetUpdate(true);
    }

    private Button GetFirstButtonOrDefault()
    {
        return GetComponentsInChildren<Button>()
            .Where(b => b.IsInteractable())
            .OrderByDescending(b => b.transform.localPosition.y) // top to bottom
            .ThenBy(b => MathF.Abs(b.transform.localPosition.x)) // middle to sides
            .FirstOrDefault();
    }
}
