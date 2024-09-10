using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EventSystemMouseFocusLossPreventer : Singleton<EventSystemMouseFocusLossPreventer>
{
    private GameObject _lastSelectedObject;

    private void Update()
    {
        // Similar to https://gamedev.stackexchange.com/a/188291
        // TODO: This is a little slow - could consider using Input system to detect mouse clicks instead?
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            _lastSelectedObject = EventSystem.current.currentSelectedGameObject;
            return;
        }

        if (!(Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            || _lastSelectedObject == null 
            || !_lastSelectedObject.activeSelf)
            return;

        var button = _lastSelectedObject.GetComponent<Button>();
        if (button == null || !button.interactable)
            return;

        Debug.Log($"Focus lost due to mouse - returning focus to last selected object {_lastSelectedObject.name}");

        EventSystem.current.SetSelectedGameObject(_lastSelectedObject);
    }
}