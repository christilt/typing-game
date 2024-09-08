using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuPage : MonoBehaviour
{
    [SerializeField] private MenuPage _nextPage;
    private MenuPage _previousPage;
    private Button _firstButton;

    private void OnEnable()
    {
        _firstButton ??= GetFirstButtonOrDefault();
        if (_firstButton == null)
        {
            Debug.LogWarning("No buttons to select");
            return;
        }

        _firstButton.Select();
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

    private void OpenFromPrevious(MenuPage previous)
    {
        if (_previousPage == null)
            _previousPage = previous;

        gameObject.SetActive(true);

        _previousPage.gameObject.SetActive(false);
    }

    private void OpenFromNext()
    {
        gameObject.SetActive(true);

        _nextPage.gameObject.SetActive(false);
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
