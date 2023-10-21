using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SequenceManager _sequenceManager;

    [SerializeField] private ScenesManager _scenesManager;

    private PauseHelper _pauseHelper;

    public event Action<GameState> OnStateChanging;
    public event Action<GameState> OnStateChanged;

    public GameState State { get; private set; }

    public void LevelWinning()
    {
        // TODO different objectives?
        TryChangeState(GameState.LevelWinning);
    }

    public void PlayerDying()
    {
        TryChangeState(GameState.PlayerDying);
    }

    public void PlayerExploding()
    {
        if (!ValidateOperation(nameof(PlayerExploding), state => state.InvolvesLevelLosing()))
            return;

        _sequenceManager.PlayerExploding();
        TryChangeState(GameState.LifeLosing);
    }

    public void PlayerExploded()
    {
        if (!ValidateOperation(nameof(PlayerExploded), state => state.InvolvesLevelLosing()))
            return;

        // TODO
    }

    private void Start()
    {
        _pauseHelper = new();
        TryChangeState(GameState.LevelStarting);
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
            case GameState.LevelStarting:
                _pauseHelper.Pause();
                break;
            case GameState.LevelPlaying:
                _pauseHelper.Unpause();
                break;
            case GameState.LevelWinning:
                _pauseHelper.Slow();
                _sequenceManager.LevelWinning();
                // TODO
                this.DoAfterSecondsRealtime(2, () => TryChangeState(GameState.LevelWon));
                break;
            case GameState.PlayerDying:
                _pauseHelper.Slow();
                _sequenceManager.PlayerDying();
                break;
            case GameState.LifeLosing:
                // TODO
                TryChangeState(GameState.LevelLost);
                break;
            case GameState.LevelWon:
                break;
            case GameState.LevelLosing:
                break;
            case GameState.LevelLost:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.LevelStarting:
                if (Input.anyKeyDown)
                {
                    TryChangeState(GameState.LevelPlaying);
                }
                break;
            case GameState.LevelPlaying:
                break;
            case GameState.LevelWinning:
                break;
            case GameState.PlayerDying:
                break;
            case GameState.LifeLosing:
                break;
            case GameState.LevelWon:
                if (Input.anyKeyDown)
                {
                    _scenesManager.ReloadLevel();
                }
                break;
            case GameState.LevelLosing:
                break;
            case GameState.LevelLost:
                if (Input.anyKeyDown)
                {
                    _scenesManager.ReloadLevel();
                }
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
    LevelStarting,
    LevelPlaying,
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
}