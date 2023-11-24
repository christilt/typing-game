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
            case GameState.LevelStarting:
                _textOverlay.ShowIntroText("Get ready");
                break;
            case GameState.LevelPlaying:
                _textOverlay.HideTextIfShown();
                break;
            case GameState.LevelWon:
                var text = GetWinText();
                _textOverlay.ShowPositiveText(text, useOverlay: false);
                break;
            case GameState.LevelLost:
                _textOverlay.ShowNegativeText("LOSE", useOverlay: false);
                break;
            default:
                _textOverlay.HideTextIfShown();
                break;
        }
    }

    // TODO
    private string GetWinText()
    {
        var settings = SettingsManager.Instance.LevelSettings;
        var stats = StatsManager.Instance.CalculateEndOfLevelStats();
        var builder = new StringBuilder();
        builder.Append(settings.LevelName);
        builder.AppendLine(" complete!");
        builder.AppendLine();
        //builder.Append("<align=\"flush\">");
        //builder.AppendLine($"Grade: {stats.Grade}");
        //builder.AppendLine($"Time: {stats.Speed.TimeTaken:mm\\:ss}");
        //builder.AppendLine($"Accuracy: {stats.Accuracy.Proportion.Value:P1}");
        //builder.Append("</align>");

        AppendLeftAndRightLine("Grade:", stats.Grade.ToString());
        AppendLeftAndRightLine("Time:", stats.Speed.TimeTaken.ToString("mm\\:ss"));
        AppendLeftAndRightLine("Accuracy:", stats.Accuracy.Proportion.Value.ToString("P1"));
        return builder.ToString();

        // See https://forum.unity.com/threads/textmeshpro-right-and-left-align-on-same-line.485157/
        void AppendLeftAndRightLine(string left, string right)
        {
            builder.Append("<align=left>");
            builder.Append(left);
            builder.AppendLine("</align>");
            builder.Append("<line-height=0><align=right>");
            builder.Append(right);
            builder.AppendLine("</align><line-height=1em>");
        }
        //void AppendLeftAndRightLine(string left, string right)
        //{
        //    builder.Append("<align=left>");
        //    builder.Append(left);
        //    builder.AppendLine("</align>");
        //    builder.Append("<line-height=0><align=right>");
        //    builder.Append(right);
        //    builder.AppendLine("</align><line-height=1em>");
        //}
        // See https://forum.unity.com/threads/textmeshpro-right-and-left-align-on-same-line.485157/
        //void AppendLeftAndRightLine(string left, string right)
        //{
        //    builder.Append("<align=left>");
        //    builder.Append(left);
        //    builder.Append("</align><line-height=0.001><align=right>");
        //    builder.Append(right);
        //    builder.AppendLine("</align>");
        //}
    }
}