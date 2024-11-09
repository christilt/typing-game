using DG.Tweening;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Hud : MonoBehaviour
{
    [SerializeField] private UIStatusEffectPanel _statusEffectPanel;
    [SerializeField] private UIStreakPopUp _streakPopUp;
    [SerializeField] private UIBurstPopUp _burstPopUp;
    [SerializeField] private UIAttackBar _attackBar;

    [SerializeField] private Hider _hider;
    [SerializeField] private MenuPage _introPage;
    [SerializeField] private MenuPage _levelCompleteMenu;
    [SerializeField] private MenuPage _levelLostPage;
    [SerializeField] private MenuPage _pauseMenu;

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

    public void Unpause() => GameplayManager.Instance.LevelUnpausing();
    public void StartLevel()
    {
        SoundManager.Instance.PlayMenuMove();
        GameplayManager.Instance.LevelGameplayStarting();
    }

    public void NextLevel()
    {
        SoundManager.Instance.PlayMenuComplete();
        LoadSceneManager.Instance.LoadNextLevel();
    }

    public void RetryLevel()
    {
        SoundManager.Instance.PlayMenuComplete();
        LoadSceneManager.Instance.ReloadLevel();
    }

    public void MainMenu()
    {
        SoundManager.Instance.PlayMenuComplete();
        LoadSceneManager.Instance.StartLoad(SceneNames.MainMenu);
    }

    private void UpdateForGameState(GameState state)
    {
        UpdateMenusForGameState(state);

        if (state.StartsGameplay())
        {
            _statusEffectPanel.gameObject.SetActive(true);
            _burstPopUp.gameObject.SetActive(true);
            _streakPopUp.gameObject.SetActive(true);
            if (_attackBar != null)
                _attackBar.gameObject.SetActive(true);
        }

        if (state.EndsGameplay())
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
            case GameState.LevelGameplayStarting:
                _introPage.Disable();
                _hider.Unhide(GameSettingsManager.Instance.MenuTransitions.FadeOutDuration);
                break;
            case GameState.LevelPausing:
                _hider.TransitionToOpaque(0, onComplete: () => _pauseMenu.gameObject.SetActive(true), unscaled: true);
                break;
            case GameState.LevelUnpausing:
                _hider.Unhide(0, onComplete: () => _pauseMenu.gameObject.SetActive(false), unscaled: true);
                break;
            case GameState.LevelWon:
                _levelCompleteMenu.gameObject.SetActive(true);
                break;
            case GameState.LevelLost:
                _levelLostPage.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}