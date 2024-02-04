using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(UINotificationPopUp))]
public class UIBurstPopUp : MonoBehaviour
{
    private UINotificationPopUp _notificationPopUp;

    private void Awake()
    {
        _notificationPopUp = GetComponent<UINotificationPopUp>();
    }

    public void Notify(BurstStat burst)
    {
        var colour = burst.Category.GetColour();
        var floorWpm = Math.Floor(burst.WordsPerMinute); // Because if rounded this same rounding would not occur later
        var text = TextHelper.WithColour($"{floorWpm} WPM", colour);
        _notificationPopUp.ShowText(text);
    }

    public void HandleReset()
    {
        _notificationPopUp.HideText();
    }
}