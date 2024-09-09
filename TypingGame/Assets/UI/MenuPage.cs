using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuPage : MonoBehaviour
{
    [SerializeField] private MenuPage _nextPage;
    private MenuPage _previousPage;
    private Button _firstButton;

    private Transitioner _optionalTransitioner;

    private void Awake()
    {
        _optionalTransitioner = GetComponent<Transitioner>();
    }

    private void OnEnable()
    {
        _firstButton ??= GetFirstButtonOrDefault();
        if (_firstButton == null)
        {
            Debug.LogWarning("No buttons to select");
            return;
        }

        _firstButton.Select();

        _optionalTransitioner?.TransitionIn();
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

    // TODO: Added for test - Remove
    public void DoDisable() => Disable();

    public Tween Disable()
    {
        if (_optionalTransitioner != null)
        {
            return _optionalTransitioner
                .TransitionOut()
                .OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);

            return DOTween.Sequence(); // TODO: Find a way to return an completed Tween?
        }
    }

    private void OpenFromPrevious(MenuPage previous)
    {
        if (_previousPage == null)
            _previousPage = previous;

        gameObject.SetActive(true);

        _previousPage.Disable();
    }

    private void OpenFromNext()
    {
        gameObject.SetActive(true);

        _nextPage.Disable();
    }

    private Button GetFirstButtonOrDefault()
    {
        return GetComponentsInChildren<Button>()
            .Where(b => b.IsInteractable())
            .OrderByDescending(b => b.transform.position.y) // top to bottom
            .ThenBy(b => MathF.Abs(b.transform.position.x)) // middle to sides
            .FirstOrDefault();
    }
}
