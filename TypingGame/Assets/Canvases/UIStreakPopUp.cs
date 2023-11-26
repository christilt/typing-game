using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(UINotificationPopUp))]
public class UIStreakPopUp : MonoBehaviour
{
    [SerializeField] private int _notifyAfterMinimumCount;
    [SerializeField] private int _notifyAfterIntervalCount;

    private UINotificationPopUp _notificationPopUp;

    private void Awake()
    {
        _notificationPopUp = GetComponent<UINotificationPopUp>();
    }

    public void MaybeNotifyOfIncrease(int streakCount)
    {
        if (streakCount < _notifyAfterMinimumCount)
            return;

        if (streakCount % _notifyAfterIntervalCount != 0)
            return;

        var colour = GetStreakColour(streakCount);
        var text = TextHelper.WithColour($"Streak x{streakCount}", colour);
        _notificationPopUp.ShowText(text);
    }

    public void HandleReset()
    {
        _notificationPopUp.HideText();
    }

    private Color GetStreakColour(int streakCount)
    {
        if (streakCount > 25)
            return SettingsManager.Instance.Palette.GreatColour;
        else
            return SettingsManager.Instance.Palette.GoodColour;
    }
}