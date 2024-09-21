using DG.Tweening;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class Hud : MonoBehaviour
{
    [SerializeField] private UIStatusEffectPanel _statusEffectPanel;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UIStreakPopUp _streakPopUp;
    [SerializeField] private UIBurstPopUp _burstPopUp;
    [SerializeField] private UIAttackBar _attackBar;

    [SerializeField] private Hider _hider;
    [SerializeField] private MenuPage _introPage;
    [SerializeField] private MenuPage _levelCompleteMenu;
    [SerializeField] private MenuPage _levelLostPage;

    private void Awake()
    {
        _canvas.worldCamera = Camera.main;
        _canvas.sortingLayerID = GetComponent<SortingGroup>().sortingLayerID;
    }

    private void Start()
    {
        UpdateForGameState(GameplayManager.Instance.State);
        GameplayManager.Instance.OnStateChanging += UpdateForGameState;

        CollectableEffectManager.Instance.OnCollectableEffectAdded += _statusEffectPanel.AddEffect;
        CollectableEffectManager.Instance.OnCollectableEffectUpdate += _statusEffectPanel.UpdateEffect;
        CollectableEffectManager.Instance.OnCollectableEffectRemoved += _statusEffectPanel.RemoveEffect;

        StatsManager.Instance.TypingRecorder.OnStreakNotification += _streakPopUp.Notify;
        StatsManager.Instance.TypingRecorder.OnStreakReset += _streakPopUp.HandleReset;

        StatsManager.Instance.TypingRecorder.OnBurstNotification += _burstPopUp.Notify;
        StatsManager.Instance.TypingRecorder.OnBurstReset += _burstPopUp.HandleReset;

        if (LevelSettingsManager.Instance.PlayerAttackSetting.Value.IsDisplayable())
            PlayerAttackManager.Instance.OnReadinessChanged += _attackBar.UpdateReadiness;
        else
            Destroy(_attackBar.gameObject);
    }

    private void OnDestroy()
    {
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnStateChanging -= UpdateForGameState;
        }

        if (CollectableEffectManager.Instance != null)
        {
            CollectableEffectManager.Instance.OnCollectableEffectAdded -= _statusEffectPanel.AddEffect;
            CollectableEffectManager.Instance.OnCollectableEffectUpdate -= _statusEffectPanel.UpdateEffect;
            CollectableEffectManager.Instance.OnCollectableEffectRemoved -= _statusEffectPanel.RemoveEffect;
        }

        if (StatsManager.Instance?.TypingRecorder != null)
        {
            StatsManager.Instance.TypingRecorder.OnStreakNotification -= _streakPopUp.Notify;
            StatsManager.Instance.TypingRecorder.OnStreakReset -= _streakPopUp.HandleReset;

            StatsManager.Instance.TypingRecorder.OnBurstNotification -= _burstPopUp.Notify;
            StatsManager.Instance.TypingRecorder.OnBurstReset -= _burstPopUp.HandleReset;
        }

        if (PlayerAttackManager.Instance != null)
        {
            PlayerAttackManager.Instance.OnReadinessChanged -= _attackBar.UpdateReadiness;
        }
    }

    public void StartLevel() => GameplayManager.Instance.LevelPLaying();
    public void NextLevel() => LoadSceneManager.Instance.LoadNextLevel();
    public void RetryLevel() => LoadSceneManager.Instance.ReloadLevel();
    public void MainMenu() => LoadSceneManager.Instance.StartLoad(SceneNames.MainMenu);

    private void UpdateForGameState(GameState state)
    {
        UpdateMenusForGameState(state);

        if (state.StartsPlayerControl())
        {
            _statusEffectPanel.gameObject.SetActive(true);
            _burstPopUp.gameObject.SetActive(true);
            _streakPopUp.gameObject.SetActive(true);
            if (_attackBar != null)
                _attackBar.gameObject.SetActive(true);
        }

        if (state.EndsPlayerControl())
        {
            _statusEffectPanel.gameObject.SetActive(false);
            _burstPopUp.gameObject.SetActive(false);
            _streakPopUp.gameObject.SetActive(false);
            if (_attackBar != null)
                _attackBar.gameObject.SetActive(false);
        }
    }

    private void UpdateMenusForGameState(GameState state)
    {
        switch (state)
        {
            case GameState.LevelIntroducing:
                _introPage.gameObject.SetActive(true);
                break;
            case GameState.LevelPlaying:
                _introPage.Disable();
                _hider.Unhide(GameSettingsManager.Instance.MenuTransitions.FadeOutDuration);
                break;
            case GameState.LevelWon:
                _levelCompleteMenu.gameObject.SetActive(true);
                // TODO: Remove
                var levelStats = StatsManager.Instance.CalculateEndOfLevelStats();
                SaveDataManager.Instance.SaveLevelHighScore(LevelSettingsManager.Instance.LevelSettings.LevelId, GameSettingsManager.Instance.Difficulty.Difficulty, "cjt", levelStats);
                break;
            case GameState.LevelLost:
                _levelLostPage.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}