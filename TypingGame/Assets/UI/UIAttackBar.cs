using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIAttackBar : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private GameObject _readyText;

    private float _fillWidth;

    private void Start()
    {
        // rectTransform.width sometimes = 0 at start, so offsetMax needed instead in those cases.  offsetMax changes so value should be recorded and max taken
        _fillWidth = Math.Max((_fill.rectTransform.offsetMax).x * -1f, _fill.rectTransform.rect.width); 
    }

    private void OnEnable()
    {
        UpdateReadiness(PlayerAttackManager.Instance.Readiness);
    }

    public void UpdateReadiness(PlayerAttackReadiness attackReadiness)
    {
        if (attackReadiness.IsReady)
        {
            _readyText.SetActive(true);
        }
        else
        {
            _readyText.SetActive(false);
        }

        // See https://stackoverflow.com/questions/30782829/how-to-access-recttransforms-left-right-top-bottom-positions-via-code
        // Right = -1 * offsetMax.x
        var x = GetFillRight(attackReadiness.Proportion) * -1f;
        Debug.Log($"Attackbar x={x}");
        _fill.rectTransform.offsetMax = new Vector2(x, _fill.rectTransform.offsetMax.y);
    }

    // If width = 400, 400 is 0% and 0 is 100%
    private float GetFillRight(float readinessProportion)
    {
        var fillMultiplier = _fillWidth / 100f;
        var readinessWidth = fillMultiplier * readinessProportion * 100f;
        return _fillWidth - readinessWidth;
    }
}