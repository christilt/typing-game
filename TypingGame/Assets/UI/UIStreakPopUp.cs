using UnityEngine;

[RequireComponent(typeof(UINotificationPopUp))]
public class UIStreakPopUp : MonoBehaviour
{
    private UINotificationPopUp _notificationPopUp;

    private void Awake()
    {
        _notificationPopUp = GetComponent<UINotificationPopUp>();
    }

    public void Notify(StreakStat streak)
    {
        var colour = streak.Category.GetColour();
        var text = TextHelper.WithColour($"Streak x{streak.Count}", colour);
        SoundManager.Instance.PlayAchievement();
        _notificationPopUp.ShowText(text);
    }

    public void HandleReset()
    {
        _notificationPopUp.HideText();
    }
}