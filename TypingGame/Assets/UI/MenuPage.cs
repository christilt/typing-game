using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuPage : MonoBehaviour
{
    [SerializeField] protected MenuPage _nextPage;
    [SerializeField] protected bool _skipTransitionOnFirstEnable;

    protected MenuPage _previousPage;
    protected Selectable[] _orderedSelectables;
    protected Selectable _firstSelectable;
    protected int _enableCount;

    protected Transitioner _optionalTransitioner;

    protected void Awake()
    {
        _optionalTransitioner = GetComponent<Transitioner>();
    }

    protected virtual void OnEnable()
    {
        var isFirstEnable = _enableCount == 0;
        _enableCount++;

        _orderedSelectables ??= GetOrderedSeletables();
        _firstSelectable ??= _orderedSelectables.FirstOrDefault();

        if (_optionalTransitioner != null && (!isFirstEnable  || !_skipTransitionOnFirstEnable))
        {
            var tween = isFirstEnable
                ? _optionalTransitioner.TransitionIn()
                : _optionalTransitioner.TransitionInRepeated();

            tween.OnComplete(() => _firstSelectable?.Select());
        }
        else
        {
            _firstSelectable?.Select();
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

    protected void OpenFromPrevious(MenuPage previous)
    {
        if (_previousPage == null)
            _previousPage = previous;

        DOTween.Sequence()
            .Append(_previousPage.Disable())
            .OnComplete(() => gameObject.SetActive(true))
            .SetUpdate(true);
    }

    protected void OpenFromNext()
    {
        DOTween.Sequence()
            .Append(_nextPage.Disable())
            .OnComplete(() => gameObject.SetActive(true))
            .SetUpdate(true);
    }

    protected Selectable[] GetOrderedSeletables()
    {
        return GetComponentsInChildren<Selectable>()
            .Where(b => b.IsInteractable())
            .OrderByDescending(b => b.transform.position.y) // top to bottom
            .ThenBy(b => MathF.Abs(b.transform.position.x)) // middle to sides
            .ToArray();
    }
}
