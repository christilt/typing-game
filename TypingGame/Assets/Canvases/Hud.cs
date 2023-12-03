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
        _statsShown = false; // TODO remove
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

    // TODO: Find how people normally do this - do these use states or different menus?
    private bool _statsShown = false;
    private void Update()
    {
        if (GameManager.Instance.State != GameState.LevelWon)
            return;

        if (Input.anyKeyDown)
        {
            if (_statsShown)
            {
                LoadingManager.Instance.ReloadLevel();
            }
            else
            {
                var (statsText1, statsText2) = GetStatsText();
                _textOverlay.FadeSwapText(statsText1, statsText2);
                _statsShown = true;
            }
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
                var (introText1, introText2) = GetIntroText();
                _textOverlay.MoveInIntroText(introText1, introText2);
                break;
            case GameState.LevelPlaying:
                _textOverlay.MoveOutTextIfShown();
                break;
            case GameState.LevelWon:
                var (winText1, winText2) = GetWinText();
                _textOverlay.MoveInPositiveText(winText1, winText2, useOverlay: false);
                break;
            case GameState.LevelLost:
                _textOverlay.MoveInNegativeText("", "Try again", useOverlay: false);
                break;
            default:
                break;
        }
    }

    private (string, string) GetIntroText()
    {
        return (SettingsManager.Instance.LevelSettings.LevelName, "Get ready");
    }

    private (string, string) GetWinText()
    {
        const int TextAlignment = 8;
        var settings = SettingsManager.Instance.LevelSettings;
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();
        var rankText = WithColour(stats, $"{stats.Rank,TextAlignment}");

        var text1 = $"{settings.LevelName}  complete!";

        var builder = new StringBuilder();
        builder.AppendLine($"{"Rank",-TextAlignment}{rankText}");
        var text2 = builder.ToString();

        return (text1, text2);
    }

    private (string, string) GetStatsText()
    {
        const int TextAlignment = 15;
        var settings = SettingsManager.Instance.LevelSettings;
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();
        var rankText = WithColour(stats, $"{stats.Rank,TextAlignment}");
        var timeText = WithColour(stats.Speed, $"{stats.Speed.TimeTaken,TextAlignment:mm\\:ss}");
        var accuracyText = WithColour(stats.Accuracy, $"{stats.Accuracy.Proportion,TextAlignment:P1}");
        // TODO
        var streakText = TextHelper.WithColour($"{42,TextAlignment}", GetCategoryColour(StatCategory.Great));
        var topSpeedText = TextHelper.WithColour($"{"32.5 WPM",TextAlignment}", GetCategoryColour(StatCategory.Good));
        var nearMissesText = $"{"3",TextAlignment}";

        var builder = new StringBuilder();
        builder.AppendLine($"{"Rank",-TextAlignment}{rankText}");
        builder.AppendLine($"{"Time",-TextAlignment}{timeText}");
        builder.AppendLine($"{"Accuracy",-TextAlignment}{accuracyText}");
        var text1 = builder.ToString();

        builder.Clear();
        builder.AppendLine();
        builder.AppendLine($"{"Best streak",-TextAlignment}{streakText}");
        builder.AppendLine($"{"Top speed",-TextAlignment}{topSpeedText}");
        builder.AppendLine($"{"Near misses",-TextAlignment}{nearMissesText}");
        var text2 = builder.ToString();

        return (text1, text2);
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