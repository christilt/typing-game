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
    [SerializeField] private float _notifyIntervalSeconds;

    private float _notifyIntervalSecondsRemaining;

    private UINotificationPopUp _notificationPopUp;

    private void Awake()
    {
        _notificationPopUp = GetComponent<UINotificationPopUp>();
    }

    private void Update()
    {
        _notifyIntervalSecondsRemaining -= Time.deltaTime;
    }

    public void MaybeNotifyOfStat(BurstStat burst)
    {
        if (_notifyIntervalSecondsRemaining > 0)
            return;

        if (!_notifyForCategories.Contains(burst.Category))
            return;

        var colour = burst.Category.GetColour();
        var floorWpm = Math.Floor(burst.WordsPerMinute); // Because if rounded this same rounding would not occur later
        var text = TextHelper.WithColour($"{floorWpm} WPM", colour);
        _notificationPopUp.ShowText(text);

        _notifyIntervalSecondsRemaining = _notifyIntervalSeconds;
    }

    public void HandleReset()
    {
        _notificationPopUp.HideText();
    }
}