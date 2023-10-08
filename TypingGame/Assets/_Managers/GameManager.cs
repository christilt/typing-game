using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action<GameState> StateChanging;
    public event Action<GameState> StateChanged;

    public GameState State { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        TryChangeState(GameState.LevelStarting);
    }

    private bool TryChangeState(GameState state)
    {
        if (State == state && State != default) 
            return false;

        StateChanging?.Invoke(state);

        State = state;
        Debug.Log($"New state: {state}");
        switch (state)
        {
            case GameState.LevelStarting:
                UnitsManager.Instance.SpawnUnits();
                TryChangeState(GameState.LevelPlaying);
                break;
            case GameState.LevelPlaying:
                break;
            default:
                Debug.LogWarning($"Unknown state rejected: {state}");
                return false;
        }

        StateChanged?.Invoke(state);


        return true;
    }
}

public enum GameState
{
    LevelStarting,
    LevelPlaying
}