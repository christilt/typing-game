using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private UIStatusEffectPanel _statusEffectPanel;

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
                _text.text = "Get ready";
                break;
            case GameState.LevelPlaying:
                _text.text = string.Empty;
                break;
            case GameState.LevelWon:
                _text.text = "WIN";
                break;
            case GameState.LevelLost:
                _text.text = "LOSE";
                break;
            default:
                _text.text = string.Empty;
                break;
        }
    }
}