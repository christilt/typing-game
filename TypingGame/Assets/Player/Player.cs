using Cinemachine;
using System;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
public class Player : Singleton<Player>
{
    [SerializeField] private PlayerVisual _visual;
    [SerializeField] private PlayerTypingMovement _typingMovement;
    [SerializeField] private Collider2D _collider;

    private void Start()
    {
        GameManager.Instance.OnStateChanging += HandleGameStateChanging;
        _visual.OnPacmanExploding += HandlePacmanExploding;
        _visual.OnPacmanExploded += HandlePacmanExploded;
    }

    public event Action<PlayerState> OnStateChanging;
    public event Action<PlayerState> OnStateChanged;

    public PlayerState State { get; private set; }

    public Vector2 Centre => _visual.transform.position;

    public void SetAsFollow(CinemachineVirtualCamera camera) => camera.Follow = _visual.transform;

    public void BecomeInvincible(float durationSeconds)
    {
        TryChangeState(PlayerState.Invincible, durationSeconds);
    }
    public void BecomeGreedy(float durationSeconds)
    {
        TryChangeState(PlayerState.Greedy, durationSeconds);
    }

    public void HitEnemy(Enemy enemy)
    {
        if (State == PlayerState.Invincible)
        {
            return;
        }

        if (State == PlayerState.Greedy)
        {
            enemy.BeDestroyed();
            return;
        }

        if (TryChangeState(PlayerState.Dying))
        {
            GameManager.Instance.PlayerDying();
        }
    }

    private void Celebrate()
    {
        TryChangeState(PlayerState.Celebrating);
    }

    private bool TryChangeState(PlayerState state, float? revertAfterSeconds = null)
    {
        if (State == state && State != default)
            return false;

        if (!Enum.IsDefined(typeof(PlayerState), state))
            throw new ArgumentOutOfRangeException(nameof(state), state, null);

        if (revertAfterSeconds.HasValue && !state.IsRevertable())
            throw new ArgumentException($"Cannot specify {nameof(revertAfterSeconds)} with state {state} as not revertable");

        StopPendingStateChanges();

        OnStateChanging?.Invoke(state);

        State = state;
        switch (state)
        {
            case PlayerState.Initial:
                SetMoveableAndCollidable(false);
                break;
            case PlayerState.Normal:
                SetMoveableAndCollidable(true);
                break;
            case PlayerState.Dying:
                SetMoveableAndCollidable(false);
                break;
            case PlayerState.Celebrating:
                SetMoveableAndCollidable(false);
                break;
            case PlayerState.Invincible:
                SetMoveableAndCollidable(true);
                break;
            case PlayerState.Greedy:
                SetMoveableAndCollidable(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        if (revertAfterSeconds != null)
            this.DoAfterSeconds(revertAfterSeconds.Value, () => TryChangeState(PlayerState.Normal));

        OnStateChanged?.Invoke(state);

        return true;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanging -= HandleGameStateChanging;
        if (_visual != null)
            _visual.OnPacmanExploding -= HandlePacmanExploding;
    }

    private void HandleGameStateChanging(GameState state)
    {
        switch (state)
        {
            case GameState.LevelPlaying:
                TryChangeState(PlayerState.Normal);
                break;
            case GameState.LevelWinning:
                Celebrate();
                break;
            default:
                break; // Do nothing for other state changes
        }
    }

    private void HandlePacmanExploding()
    {
        GameManager.Instance.PlayerExploding();
    }

    private void HandlePacmanExploded()
    {
        GameManager.Instance.PlayerExploded();
        gameObject.SetActive(false);
    }

    private void StopPendingStateChanges()
    {
        StopAllCoroutines();
    }

    private void SetMoveableAndCollidable(bool enabled)
    {
        SetMoveable(enabled);

        _collider.enabled = enabled;
    }

    private void SetMoveable(bool enabled)
    {
        if (enabled)
            _typingMovement.EnableComponent();
        else
            _typingMovement.DisableComponent();
    }
}

public enum PlayerState
{
    Initial,
    Normal,
    Invincible,
    Greedy,
    Dying,
    Celebrating
}

public static class PlayerStateExtensions
{
    public static bool IsEffect(this PlayerState state)
    {
        return state == PlayerState.Invincible
            || state == PlayerState.Greedy;
    }

    public static bool IsRevertable(this PlayerState state)
    {
        return state.IsEffect();  
    }
}