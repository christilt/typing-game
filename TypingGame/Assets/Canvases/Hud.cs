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
    [SerializeField] private Color _greatColor;
    [SerializeField] private Color _goodColor;
    [SerializeField] private Color _averageColor;
    [SerializeField] private Color _badColor;

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
    }

    private void UpdateForGameState(GameState state)
    {
        UpdateTextForGameState(state);

        if (state.EndsPlayerControl())
        {
            _statusEffectPanel.gameObject.SetActive(false);
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
        var rankText = WithColor(stats, $"{stats.Rank,Alignment}");
        var timeText = WithColor(stats.Speed, $"{stats.Speed.TimeTaken, Alignment:mm\\:ss}");
        var accuracyText = WithColor(stats.Accuracy, $"{stats.Accuracy.Proportion, Alignment:P1}");

        var builder = new StringBuilder();
        builder.Append(settings.LevelName);
        builder.AppendLine("  complete!");
        builder.AppendLine();
        builder.AppendLine($"{"Rank:", -Alignment}{rankText}");
        builder.AppendLine($"{"Time:", -Alignment}{timeText}");
        builder.AppendLine($"{"Accuracy:", -Alignment}{accuracyText}");
        return builder.ToString();
    }

    private string WithColor(IStat stat, string formattedText)
    {
        var color = GetCategoryColor(stat.Category);
        var colorString = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorString}>{formattedText}</color>";
    }

    private Color GetCategoryColor(StatCategory category)
    {
        switch (category)
        {
            case StatCategory.Great:
                return _greatColor;
            case StatCategory.Good:
                return _goodColor;
            case StatCategory.Average:
                return _averageColor;
            default:
                return _badColor;
        }
    }
}