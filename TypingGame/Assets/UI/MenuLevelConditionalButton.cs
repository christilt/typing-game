using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuLevelConditionalButton : MonoBehaviour
{
    [SerializeField] private LevelSettingsSO _levelSettings;
    [SerializeField] private bool _isAlwaysInteractable;
    public LevelSettingsSO LevelSettings => _levelSettings;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void MaybeSetInteractable(GameProgress progress)
    {
        if (_isAlwaysInteractable)
        {
            _button.interactable = true;
            return;
        }

        if (_levelSettings == null)
        {
            return;
        }

        var difficultyKey = GameSettingsManager.Instance.Difficulty.Difficulty.ToString();
        if (progress.DifficultyHighestReachedLevels.TryGetValue(difficultyKey, out var highestReachedLevelOrder) && highestReachedLevelOrder >= _levelSettings.LevelOrder)
        {
            _button.interactable = true;
        }
        else
        {
            _button.interactable = false;
        }
    }
}