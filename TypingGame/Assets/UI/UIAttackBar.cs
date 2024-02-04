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
        _fillWidth = (_fill.rectTransform.offsetMax).x * -1; // rectTransform.width = 0 at start, so offsetMax needed instead.  offsetMax changes so value should be recorded
        PlayerAttackManager.Instance.OnReadinessChanged += UpdateReadiness;
    }

    private void OnDestroy()
    {
        PlayerAttackManager.Instance.OnReadinessChanged -= UpdateReadiness;
    }

    private void UpdateReadiness(float increase)
    {
        if (PlayerAttackManager.Instance.IsReady)
        {
            _readyText.SetActive(true);
        }
        else
        {
            _readyText.SetActive(false);
        }

        // See https://stackoverflow.com/questions/30782829/how-to-access-recttransforms-left-right-top-bottom-positions-via-code
        // Right = -1 * offsetMax.x
        _fill.rectTransform.offsetMax = new Vector2(GetFillRight() * -1, _fill.rectTransform.offsetMax.y);
    }

    // If width = 400, 400 is 0% and 0 is 100%
    private float GetFillRight()
    {
        var fillMultiplier = _fillWidth / 100f;
        var readinessWidth = fillMultiplier * PlayerAttackManager.Instance.ReadinessProportion * 100;
        return _fillWidth - readinessWidth;
    }
}