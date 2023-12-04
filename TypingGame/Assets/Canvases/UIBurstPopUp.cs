using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(UINotificationPopUp))]
public class UIBurstPopUp : MonoBehaviour
{
    [SerializeField] private StatCategory[] _notifyForCategories;
    [SerializeField] private float _notifyInterval;

    private float _sincePreviousNotificationSeconds;

    private UINotificationPopUp _notificationPopUp;

    private void Awake()
    {
        _notificationPopUp = GetComponent<UINotificationPopUp>();
    }

    private void Update()
    {
        _sincePreviousNotificationSeconds -= Time.deltaTime;
    }

    public void MaybeNotifyOfIncrease(BurstStat burst)
    {
        if (_sincePreviousNotificationSeconds > 0)
            return;

        if (!_notifyForCategories.Contains(burst.Category))
            return;

        var colour = burst.Category.GetColour();
        var text = TextHelper.WithColour($"{burst.WordsPerMinute:n0} WPM", colour);
        _notificationPopUp.ShowText(text);
    }

    public void HandleReset()
    {
        _notificationPopUp.HideText();
    }
}