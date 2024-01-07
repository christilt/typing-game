using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAttackVisual : MonoBehaviour
{
    [SerializeField] private Ease _fadeEase;

    private SpriteRenderer _spriteRenderer;

    private Tween _fadeTween;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        _fadeTween?.Kill();
    }

    public void AnimateAttack(float duration)
    {
        _fadeTween?.Kill();

        _fadeTween = _spriteRenderer.DOFade(0, duration)
            .SetEase(_fadeEase);
    }
}