using DG.Tweening;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class Hud : MonoBehaviour
{
    [SerializeField] private UIStatusEffectPanel _statusEffectPanel;
    [SerializeField] private UITextOverlay _textOverlay;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UIStreakPopUp _streakPopUp;

    private void Awake()
    {
        _canvas.worldCamera = Camera.main;
        _canvas.sortingLayerID = GetComponent<SortingGroup>().sortingLayerID;
    }

    private void Start()
    {
        UpdateForGameState(GameManager.Instance.State);
        GameManager.Instance.OnStateChanging += UpdateForGameState;

        CollectableEffectManager.Instance.OnCollectableEffectAdded += _statusEffectPanel.AddEffect;
        CollectableEffectManager.Instance.OnCollectableEffectUpdate += _statusEffectPanel.UpdateEffect;
        CollectableEffectManager.Instance.OnCollectableEffectRemoved += _statusEffectPanel.RemoveEffect;

        StatsManager.Instance.TypingRecorder.OnStreakIncreased += _streakPopUp.MaybeNotifyOfIncrease;
        StatsManager.Instance.TypingRecorder.OnStreakReset += _streakPopUp.HandleReset;

    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanging -= UpdateForGameState;
        }

        if (CollectableEffectManager.Instance != null)
        {
            CollectableEffectManager.Instance.OnCollectableEffectAdded -= _statusEffectPanel.AddEffect;
            CollectableEffectManager.Instance.OnCollectableEffectUpdate -= _statusEffectPanel.UpdateEffect;
            CollectableEffectManager.Instance.OnCollectableEffectRemoved -= _statusEffectPanel.RemoveEffect;
        }

        if (StatsManager.Instance?.TypingRecorder != null)
        {
            StatsManager.Instance.TypingRecorder.OnStreakIncreased -= _streakPopUp.MaybeNotifyOfIncrease;
            StatsManager.Instance.TypingRecorder.OnStreakReset -= _streakPopUp.HandleReset;
        }
    }

    private void UpdateForGameState(GameState state)
    {
        UpdateTextForGameState(state);

        if (state.EndsPlayerControl())
        {
            _statusEffectPanel.gameObject.SetActive(false);
            _streakPopUp.gameObject.SetActive(false);
        }
    }

    private void UpdateTextForGameState(GameState state)
    {
        switch (state)
        {
            case GameState.LevelIntroducing:
                _textOverlay.ShowIntroText(GetIntroText());
                break;
            case GameState.LevelPlaying:
                _textOverlay.HideTextIfShown();
                break;
            case GameState.LevelWon:
                _textOverlay.ShowPositiveText(GetWinText(), useOverlay: false);
                break;
            case GameState.LevelLost:
                _textOverlay.ShowNegativeText("Try again", useOverlay: false);
                break;
            default:
                break;
        }
    }

    private string GetIntroText()
    {
        var builder = new StringBuilder();
        builder.AppendLine(SettingsManager.Instance.LevelSettings.LevelName);
        builder.AppendLine();
        builder.AppendLine("Get ready");
        return builder.ToString();
    }

    private string GetWinText()
    {
        const int Alignment = 10;
        var settings = SettingsManager.Instance.LevelSettings;
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();
        var rankText = WithColour(stats, $"{stats.Rank,Alignment}");
        var timeText = WithColour(stats.Speed, $"{stats.Speed.TimeTaken, Alignment:mm\\:ss}");
        var accuracyText = WithColour(stats.Accuracy, $"{stats.Accuracy.Proportion, Alignment:P1}");

        var builder = new StringBuilder();
        builder.Append(settings.LevelName);
        builder.AppendLine("  complete!");
        builder.AppendLine();
        builder.AppendLine($"{"Rank:", -Alignment}{rankText}");
        builder.AppendLine($"{"Time:", -Alignment}{timeText}");
        builder.AppendLine($"{"Accuracy:", -Alignment}{accuracyText}");
        return builder.ToString();
    }

    private string WithColour(IStat stat, string formattedText)
    {
        var colour = GetCategoryColour(stat.Category);
        return TextHelper.WithColour(formattedText, colour);
    }

    private Color GetCategoryColour(StatCategory category)
    {
        switch (category)
        {
            case StatCategory.Great:
                return SettingsManager.Instance.Palette.GreatColour;
            case StatCategory.Good:
                return SettingsManager.Instance.Palette.GoodColour;
            case StatCategory.Average:
                return SettingsManager.Instance.Palette.AverageColour;
            default:
                return SettingsManager.Instance.Palette.BadColour;
        }
    }
}