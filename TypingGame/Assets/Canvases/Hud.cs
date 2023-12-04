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
    [SerializeField] private UIBurstPopUp _burstPopUp;

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

        StatsManager.Instance.TypingRecorder.OnStreakIncreased += _streakPopUp.MaybeNotifyOfStat;
        StatsManager.Instance.TypingRecorder.OnStreakReset += _streakPopUp.HandleReset;

        StatsManager.Instance.TypingRecorder.OnBurstMeasured += _burstPopUp.MaybeNotifyOfStat;
        StatsManager.Instance.TypingRecorder.OnBurstReset += _burstPopUp.HandleReset;

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
            StatsManager.Instance.TypingRecorder.OnStreakIncreased -= _streakPopUp.MaybeNotifyOfStat;
            StatsManager.Instance.TypingRecorder.OnStreakReset -= _streakPopUp.HandleReset;

            StatsManager.Instance.TypingRecorder.OnBurstMeasured -= _burstPopUp.MaybeNotifyOfStat;
            StatsManager.Instance.TypingRecorder.OnBurstReset -= _burstPopUp.HandleReset;
        }
    }

    // TODO: Find how people normally do this - do they use states or different menus?
    private bool _statsShown = false;
    private void Update()
    {
        var state = GameManager.Instance.State;
        if (!(state.IsEndOfLevel() && Input.anyKeyDown))
        {
            return;
        }

        if (state == GameState.LevelWon && !_statsShown)
        {
            var (statsText1, statsText2) = GetStatsText();
            _textOverlay.FadeSwapText(statsText1, statsText2);
            _statsShown = true;
            return;
        }

        LoadingManager.Instance.ReloadLevel();
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
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();
        var rankText = WithColour(stats, $"{stats.Rank}");

        var text1 = $"{SettingsManager.Instance.LevelSettings.LevelName}  complete!";

        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine("Rank");
        builder.AppendLine();
        builder.AppendLine(rankText);
        var text2 = builder.ToString();

        return (text1, text2);
    }

    private (string, string) GetStatsText()
    {
        const int TextAlignment = 15;
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();

        var builder = new StringBuilder();

        builder.Append(SettingsManager.Instance.LevelSettings.LevelName);
        builder.AppendLine(" stats");

        var rankText = WithColour(stats, $"{stats.Rank,TextAlignment}");
        builder.AppendLine($"{"Rank",-TextAlignment}{rankText}");

        var accuracyText = WithColour(stats.Accuracy, $"{stats.Accuracy.Proportion,TextAlignment:P0}");
        builder.AppendLine($"{"Accuracy",-TextAlignment}{accuracyText}");
        
        var text1 = builder.ToString();
        builder.Clear();

        builder.AppendLine();

        var timeText = WithColour(stats.Speed, $"{stats.Speed.TimeTaken,TextAlignment:mm\\:ss}");
        builder.AppendLine($"{"Time",-TextAlignment}{timeText}");

        if (stats.BestStreak.Category.IsAtLeastGood())
        {
            var streakText = WithColour(stats.BestStreak, $"{stats.BestStreak.Count,TextAlignment}");
            builder.AppendLine($"{"Best streak",-TextAlignment}{streakText}");
        }

        if (stats.TopSpeed.Category.IsAtLeastGood())
        {
            var topSpeedText = TextHelper.WithColour($"{$"{stats.TopSpeed.WordsPerMinute:n1} WPM",TextAlignment}", StatCategory.Good.GetColour());
            builder.AppendLine($"{"Top speed",-TextAlignment}{topSpeedText}");
        }

        // Maybe add misc stats?

        var text2 = builder.ToString();

        return (text1, text2);
    }

    private string WithColour(IStat stat, string formattedText)
    {
        var colour = stat.Category.GetColour();
        return TextHelper.WithColour(formattedText, colour);
    }
}