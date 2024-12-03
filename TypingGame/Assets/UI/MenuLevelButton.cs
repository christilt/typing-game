using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuLevelButton : MenuButton, ISubmitHandler
{
    [SerializeField] private LevelSettingsSO _levelSettings;
    private Button _button;

    public event Action<LevelSettingsSO> OnLevelSubmit;

    protected virtual void Awake()
    {
        _button = GetComponent<Button>();  
    }

    protected virtual void Start()
    {
        if (_levelSettings == null)
        {
            return;
        }

        var progress = SaveDataManager.Instance.LoadGameProgress();
        var difficultyKey = GameSettingsManager.Instance.Difficulty.Difficulty.ToString();
        if (progress.DifficultyHighestReachedLevels.TryGetValue(difficultyKey, out var currentHighestReachedLevelOrderValue) && currentHighestReachedLevelOrderValue >= _levelSettings.LevelOrder)
        {
            _button.interactable = true;
        }
        else
        {
            // Do not make button noninteractable - allow some buttons set as interactable already to remain so
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnLevelSubmit?.Invoke(_levelSettings);
    }
}