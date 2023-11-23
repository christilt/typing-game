using DG.Tweening;
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
        // TODO remove
        Debug.Log("Hud start");
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
        // TODO remove
        Debug.Log($"Hud UpdateTextForGameState {state}");
        switch (state)
        {
            case GameState.LevelStarting:
                _textOverlay.ShowIntroText("Get ready");
                break;
            case GameState.LevelPlaying:
                _textOverlay.HideTextIfShown();
                break;
            case GameState.LevelWon:
                _textOverlay.ShowPositiveText("WIN", useOverlay: false);
                break;
            case GameState.LevelLost:
                _textOverlay.ShowNegativeText("LOSE", useOverlay: false);
                break;
            default:
                _textOverlay.HideTextIfShown();
                break;
        }
    }
}