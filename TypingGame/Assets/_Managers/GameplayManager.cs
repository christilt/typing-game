using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : Singleton<GameplayManager>
{
    [SerializeField] private SequenceDirector _sequenceDirector;
    [SerializeField] private PerformanceLogger _performanceLogger;
    [SerializeField] private Camera _mainCamera;

    private PauseHelper _pauseHelper;

    public event Action<GameState> OnStateChanging;
    public event Action<GameState> OnStateChanged;

    public GameState State { get; private set; }

    public void LoadComplete()
    {
        TryChangeState(GameState.LevelStarting);
    }

    public void LevelGameplayStarting()
    {
        TryChangeState(GameState.LevelGameplayStarting);
    }

    public void LevelWinning()
    {
        TryChangeState(GameState.LevelWinning);
    }

    public void LevelPausing()
    {
        if (!ValidateOperation(nameof(LevelPausing), state => state.AllowsPausing()))
            return;

        TryChangeState(GameState.LevelPausing);
    }

    public void LevelUnpausing()
    {
        if (!ValidateOperation(nameof(LevelUnpausing), state => state == GameState.LevelPausing))
            return;

        TryChangeState(GameState.LevelUnpausing);
    }

    public void PlayerDying()
    {
        TryChangeState(GameState.PlayerDying);
    }

    public void PlayerExploding()
    {
        if (!ValidateOperation(nameof(PlayerExploding), state => state.InvolvesLevelLosing()))
            return;

        _sequenceDirector.PlayerExploding();
        TryChangeState(GameState.LifeLosing);
    }

    public void PlayerExploded()
    {
        if (!ValidateOperation(nameof(PlayerExploded), state => state.InvolvesLevelLosing()))
            return;

    }

    public void SceneEnding(bool isExitingGameplay)
    {
        if (isExitingGameplay)
            SoundManager.Instance.StopMusicInGame();

        _pauseHelper.Unpause();
    }

    private void Start()
    {
        _pauseHelper = new();
        TryChangeState(GameState.LevelLoading);

        if (SceneHelper.IsSceneLoadedAdditively(LevelSettingsManager.Instance.LevelSettings.SceneName))
        {
            LoadSceneManager.Instance.OnLoadComplete += LoadComplete;
        }
        else
        {
            LoadComplete();
        }
    }

    private void OnDestroy()
    {
        if (LoadSceneManager.Instance != null)
        {
            LoadSceneManager.Instance.OnLoadComplete -= LoadComplete;
        }
    }

    private bool TryChangeState(GameState state)
    {
        if (State == state && State != default) 
            return false;

        if (!Enum.IsDefined(typeof(GameState), state))
            throw new ArgumentOutOfRangeException(nameof(state), state, null);

        Debug.Log($"Game state changing to: {state}");
        OnStateChanging?.Invoke(state);

        HandleNewState(state);

        OnStateChanged?.Invoke(state);

        return true;
    }

    private void HandleNewState(GameState state)
    {
        State = state;
        switch (state)
        {
            case GameState.LevelLoading:
                _pauseHelper.Pause();
                KeyTiles.Instance.Initialise();
                break;
            case GameState.LevelStarting:
                _mainCamera.gameObject.SetActive(true);
#if DEBUG
                _performanceLogger.gameObject.SetActive(true);
#endif
                SceneHider.Instance.StartOfSceneFadeIn();
                SoundManager.Instance.StartMusicInGame();
                TryChangeState(GameState.LevelIntroducing);
                break;
            case GameState.LevelIntroducing:
                break;
            case GameState.LevelGameplayStarting:
                _pauseHelper.Unpause();
                TryChangeState(GameState.LevelPlaying);
                break;
            case GameState.LevelPlaying:
                break;
            case GameState.LevelPausing:
                _pauseHelper.Pause();
                break;
            case GameState.LevelUnpausing:
                _pauseHelper.Unpause();
                TryChangeState(GameState.LevelPlaying);
                break;
            case GameState.LevelWinning:
                _pauseHelper.Slow();
                _sequenceDirector.LevelWinning(() =>
                {
                    TryChangeState(GameState.LevelWon);
                });
                break;
            case GameState.PlayerDying:
                _pauseHelper.Slow();
                _sequenceDirector.PlayerDying(() =>
                {
                    TryChangeState(GameState.LevelLost);
                });
                break;
            case GameState.LifeLosing:
                break;
            case GameState.LevelWon:
                break;
            case GameState.LevelLosing:
                break;
            case GameState.LevelLost:
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.LevelLoading:
                break;
            case GameState.LevelStarting:
                break;
            case GameState.LevelIntroducing:
                break;
            case GameState.LevelGameplayStarting:
                break;
            case GameState.LevelPlaying:
                break;
            case GameState.LevelPausing:
                break;
            case GameState.LevelUnpausing:
                break;
            case GameState.LevelWinning:
                break;
            case GameState.PlayerDying:
                break;
            case GameState.LifeLosing:
                break;
            case GameState.LevelWon:
                break;
            case GameState.LevelLosing:
                break;
            case GameState.LevelLost:
                break;
            default:
                break;
        }
    }

    private bool ValidateOperation(string operationName, Func<GameState, bool> validation)
    {
        if (!validation(State))
        {
            Debug.LogError($"Invalid to call {operationName} when State is {State}");
            return false;
        }

        return true;
    }
}

public enum GameState
{
    LevelLoading,
    LevelStarting,
    LevelIntroducing,
    LevelGameplayStarting,
    LevelPlaying,
    LevelPausing,
    LevelUnpausing,
    LevelWinning,
    LevelWon,
    PlayerDying,
    LifeLosing,
    LevelLosing,
    LevelLost
}

public static class GameStateExtensions
{
    public static bool InvolvesLevelWinning(this GameState state)
    {
        return state == GameState.LevelWinning
            || state == GameState.LevelWon;
    }

    public static bool InvolvesLevelLosing(this GameState state)
    {
        return state == GameState.PlayerDying
            || state == GameState.LifeLosing
            || state == GameState.LevelLosing
            || state == GameState.LevelLost;
    }

    public static bool StartsMenuControl(this GameState state) 
    {
        return state == GameState.LevelIntroducing;
    }

    public static bool StartsGameplay(this GameState state)
    {
        return state == GameState.LevelGameplayStarting;
    }

    public static bool EndsGameplay(this GameState state)
    {
        return state.InvolvesLevelWinning()
            || state.InvolvesLevelLosing();
    }

    public static bool AllowsPausing(this GameState state)
    {
        return state == GameState.LevelPlaying;
    }

    public static bool AllowsUnpausing(this GameState state)
    {
        return state == GameState.LevelPausing;
    }

    public static bool IsEndOfLevel(this GameState state)
    {
        return state == GameState.LevelWon
            || state == GameState.LevelLost;
    }
}