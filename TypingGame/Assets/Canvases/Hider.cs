using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(SortingGroup))]
public class Hider : MonoBehaviour
{
    [SerializeField] private Image _hiderImage;
    [SerializeField] private Canvas _hiderCanvas;
    [SerializeField, Range(0,255)] private int _startAlpha;
    private float _startAlphaFloat;
    private Tween _tween;

    private void Awake()
    {
        // TODO remove
        Debug.Log($"{name} start alpha is {_startAlpha}");
        _hiderCanvas.worldCamera = Camera.main;
        _hiderCanvas.sortingLayerID = GetComponent<SortingGroup>().sortingLayerID;
        _startAlphaFloat = _startAlpha / 255f;
        var startColour = Camera.main.backgroundColor;
        startColour.a = _startAlphaFloat;
        _hiderImage.color = startColour;
        // TODO remove
        Debug.Log($"{name} start color is {startColour}");
    }

    // TODO Show / Hide based on rate rather than duration?

    public void Unhide(float duration, Action onComplete = null, bool unscaled = false)
    {
        Fade(0, duration, onComplete, unscaled);
    }

    public void FadeToStartAlpha(float duration, Action onComplete = null, bool unscaled = false)
    {
        Fade(_startAlphaFloat, duration, onComplete, unscaled);
    }

    public void HideCompletely(float duration, Action onComplete = null, bool unscaled = false)
    {
        Fade(1, duration, onComplete, unscaled);
    }

    // TODO remove
    public void Fade(float alpha, float duration, Action onComplete = null, bool unscaled = false)
    {
        // TODO remove
        Debug.Log($"{name} set alpha from {_hiderImage.color.a} to {alpha}");
        if (_hiderImage.color.a == alpha)
            return;

        if (_tween != null)
            _tween.Kill();

        _tween = _hiderImage
            .DOFade(alpha, duration)
            .SetUpdate(unscaled);

        if (onComplete != null)
            _tween.OnComplete(() => onComplete());
    }
    //public void Fade(float alpha, float duration, Action onComplete, bool unscaled = false)
    //{
    //    StartCoroutine(FadeCoroutine());

    //    IEnumerator FadeCoroutine()
    //    {
    //        var time = 0f;
    //        var color = _hiderImage.color;
    //        var originalAlpha = color.a;
    //        while (time < duration)
    //        {
    //            time += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
    //            var lerp = time / duration;
    //            color.a = Mathf.Lerp(originalAlpha, alpha, lerp);
    //            _hiderImage.color = color;
    //            yield return null;
    //        }

    //        if (onComplete != null)
    //            onComplete();
    //    }
    //}
}