﻿using System.Collections;
using UnityEngine;

public class Hider : MonoBehaviour
{
    [SerializeField] SpriteRenderer _hiderSprite;
    [SerializeField] Canvas _hiderCanvas;

    private void Start()
    {
        _hiderCanvas.worldCamera = Camera.main;
    }

    public void Hide(float duration, bool unscaled = false)
    {
        StartCoroutine(HideCoroutine());

        IEnumerator HideCoroutine()
        {
            var time = 0f;
            var color = _hiderSprite.color;
            while (time < duration)
            {
                time += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                var lerp = time / duration;
                color.a = Mathf.Lerp(0, 1, lerp);
                _hiderSprite.color = color;
                yield return null;
            }
        }
    }
}