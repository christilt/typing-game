using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : Singleton<GameplayManager>
{
    [SerializeField] private SequenceDirector _sequenceDirector;

    private PauseHelper _pauseHelper;

    public event Action<GameState> OnStateChanging;
    public event Action<GameState> OnStateChanged;

    public GameState State { get; private set; }

    public void LevelWinning()
    {
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

        _sequenceDirector.PlayerExploding();
        TryChangeState(GameState.LifeLosing);
    }

    public void PlayerExploded()
    {
        if (!ValidateOperation(nameof(PlayerExploded), state => state.InvolvesLevelLosing()))
            return;

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
                SceneHider.Instance.StartOfSceneFadeIn();
                KeyTiles.Instance.Initialise(() =>
                {
                    TryChangeState(GameState.LevelIntroducing);
                });
                break;
            case GameState.LevelIntroducing:
                break;
            case GameState.LevelPlaying:
                _pauseHelper.Unpause();
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
                // TODO
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
            case GameState.LevelIntroducing:
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

// TODO - maybe 2 sets of states, one higher level so other components need to know less
public enum GameState
{
    LevelStarting,
    LevelIntroducing,
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

    public static bool StartsMenuControl(this GameState state) 
    {
        return state == GameState.LevelIntroducing;
    }

    public static bool StartsPlayerControl(this GameState state)
    {
        return state == GameState.LevelPlaying;
    }

    public static bool EndsPlayerControl(this GameState state)
    {
        return state.InvolvesLevelWinning()
            || state.InvolvesLevelLosing();
    }

    public static bool IsEndOfLevel(this GameState state)
    {
        return state == GameState.LevelWon
            || state == GameState.LevelLost;
    }
}