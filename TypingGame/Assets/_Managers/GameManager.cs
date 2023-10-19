using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private LevelCompletingSequence _levelCompletingSequence;
    [SerializeField] private PlayerDyingSequence _playerDyingSequence;

    [SerializeField] private SceneTransitionManager _sceneTransitionManager;

    private PauseHelper _pauseHelper;

    public event Action<GameState> OnStateChanging;
    public event Action<GameState> OnStateChanged;

    public GameState State { get; private set; }

    public void LevelCompleting()
    {
        // TODO different objectives?
        TryChangeState(GameState.LevelCompleting);
    }

    public void PlayerDying()
    {
        TryChangeState(GameState.PlayerDying);
    }

    private void Start()
    {
        _pauseHelper = new();
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
            case GameState.LevelCompleting:
                // TODO
                if (Input.anyKeyDown)
                {
                    _sceneTransitionManager.ReloadLevel();
                }
                break;
            case GameState.PlayerDying:
                // TODO
                if (Input.anyKeyDown)
                {
                    _sceneTransitionManager.ReloadLevel();
                }
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
        _pauseHelper.Pause();
    }

    private void HandleLevelPlaying()
    {
        _pauseHelper.Unpause();
    }

    private void HandleLevelCompleting()
    {
        // TODO centralise this a little?
        _pauseHelper.Slow();
        _levelCompletingSequence.Play();
    }

    private void HandlePlayerDying()
    {
        _pauseHelper.Slow();
        _playerDyingSequence.Play();
    }
}

public enum GameState
{
    LevelStarting,
    LevelPlaying,
    LevelCompleting,
    PlayerDying
}