using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAttackBar : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private GameObject _readyText;
    [SerializeField] private Color _pulseFillColor;
    [SerializeField] private float _pulseFillSeconds;
    [SerializeField] private Ease _pulseFillEase;

    private Tween _pulseWhiteTween;

    private float _fillWidth;

    private void OnEnable()
    {
        // rectTransform.width sometimes = 0 at start, so offsetMax needed instead in those cases.  offsetMax changes so value should be recorded and max taken
        _fillWidth = Math.Max((_fill.rectTransform.offsetMax).x * -1f, _fill.rectTransform.rect.width);
        UpdateReadiness(PlayerAttackManager.Instance.Readiness);
    }

    private void OnDisable()
    {
        _pulseWhiteTween?.Kill();
    }

    public void UpdateReadiness(PlayerAttackReadiness attackReadiness)
    {
        _readyText.gameObject.SetActive(attackReadiness.IsReady);
        SetPulsing(attackReadiness.IsReady);
        // See https://stackoverflow.com/questions/30782829/how-to-access-recttransforms-left-right-top-bottom-positions-via-code
        // Right = -1 * offsetMax.x
        var x = GetFillRight(attackReadiness.Proportion) * -1f;
        _fill.rectTransform.offsetMax = new Vector2(x, _fill.rectTransform.offsetMax.y);
    }

    private void SetPulsing(bool active)
    {
        _pulseWhiteTween?.Kill();

        if (active)
        {
            _pulseWhiteTween = _fill.DOColor(_pulseFillColor, _pulseFillSeconds)
                .SetEase(_pulseFillEase)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    // If width = 400, 400 is 0% and 0 is 100%
    private float GetFillRight(float readinessProportion)
    {
        var fillMultiplier = _fillWidth / 100f;
        var readinessWidth = fillMultiplier * readinessProportion * 100f;
        return _fillWidth - readinessWidth;
    }
}