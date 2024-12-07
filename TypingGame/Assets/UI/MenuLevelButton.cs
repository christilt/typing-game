using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(MenuLevelConditionalButton))]
public class MenuLevelButton : MenuButton, ISubmitHandler
{
    private MenuLevelConditionalButton _menuLevelConditionalButton;

    public event Action<LevelSettingsSO> OnLevelSubmit;

    protected virtual void Awake()
    {
        _menuLevelConditionalButton = GetComponent<MenuLevelConditionalButton>();  
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnLevelSubmit?.Invoke(_menuLevelConditionalButton.LevelSettings);
    }
}
