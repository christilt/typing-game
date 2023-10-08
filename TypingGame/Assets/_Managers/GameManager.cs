using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action<GameState> OnStateChanging;
    public event Action<GameState> OnStateChanged;

    public GameState State { get; private set; }

    private void Start()
    {
        TryChangeState(GameState.LevelStarting);
    }

    private bool TryChangeState(GameState state)
    {
        if (State == state && State != default) 
            return false;

        if (!Enum.IsDefined(typeof(GameState), state))
            throw new ArgumentOutOfRangeException(nameof(state), state, null);

        Debug.Log($"State changing to: {state}");
        OnStateChanging?.Invoke(state);

        HandleNewState(state);

        Debug.Log($"State changed to: {state}");
        OnStateChanged?.Invoke(state);

        return true;
    }

    private void HandleNewState(GameState state)
    {
        State = state;
        switch (state)
        {
            case GameState.LevelStarting:
                HandleLevelStarting();
                break;
            case GameState.LevelPlaying:
                HandleLevelPlaying();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void HandleLevelStarting()
    {
        // TODO
        PauseManager.Instance.Pause();
        this.DoAfterSecondsRealtime(3, () => TryChangeState(GameState.LevelPlaying));
    }

    private void HandleLevelPlaying()
    {
        PauseManager.Instance.Unpause();
    }
}

public enum GameState
{
    LevelStarting,
    LevelPlaying
}