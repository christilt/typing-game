using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action<GameState> OnStateChanging;
    public event Action<GameState> OnStateChanged;

    public GameState State { get; private set; }

    public void PlayerDying()
    {
        TryChangeState(GameState.PlayerDying);
    }

    private void Start()
    {
        TryChangeState(GameState.LevelStarting);
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
            default:
                break;
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
            case GameState.LevelStarting:
                HandleLevelStarting();
                break;
            case GameState.LevelPlaying:
                HandleLevelPlaying();
                break;
            case GameState.LevelCompleting:
                HandleLevelCompleting();
                break;
            case GameState.PlayerDying:
                HandlePlayerDying();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void HandleLevelStarting()
    {
        PauseManager.Instance.Pause();
    }

    private void HandleLevelPlaying()
    {
        PauseManager.Instance.Unpause();
    }

    private void HandleLevelCompleting()
    {
        PauseManager.Instance.Pause();
    }

    private void HandlePlayerDying()
    {
        PauseManager.Instance.Pause();
    }
}

public enum GameState
{
    LevelStarting,
    LevelPlaying,
    LevelCompleting,
    PlayerDying
}