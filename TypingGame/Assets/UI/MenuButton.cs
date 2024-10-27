using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IDeselectHandler
{
    public void OnDeselect(BaseEventData eventData)
    {
        SoundManager.Instance?.PlayMenuMove();
    }
}
