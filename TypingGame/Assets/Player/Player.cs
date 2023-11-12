using Cinemachine;
using System;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
public class Player : Singleton<Player>
{
    [SerializeField] private PlayerVisual _visual;
    [SerializeField] private PlayerTypingMovement _typingMovement;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _invincibleSeconds;

    private void Start()
    {
        GameManager.Instance.OnStateChanging += HandleGameStateChanging;
        _visual.OnPacmanExploding += HandlePacmanExploding;
        _visual.OnPacmanExploded += HandlePacmanExploded;
    }

    public event Action<PlayerState> OnStateChanging;
    public event Action<PlayerState> OnStateChanged;

    public event Action<PlayerEffect?> OnEffectChanging;
    public event Action<PlayerEffect?> OnEffectChanged;

    public PlayerState State { get; private set; }
    public PlayerEffect? Effect { get; private set; }

    public Vector2 Centre => _visual.transform.position;

    public void SetAsFollow(CinemachineVirtualCamera camera) => camera.Follow = _visual.transform;

    public void BecomeInvincible(float duration)
    {
        TryChangeEffect(PlayerEffect.Invincible, duration);
    }

    public void HitEnemy(Enemy enemy)
    {
        if (Effect == PlayerEffect.Invincible)
        {
            return;
        }

        if (Effect == PlayerEffect.Greedy)
        {
            enemy.BeDestroyed();
            return;
        }

        if (TryChangeState(PlayerState.Dying))
        {
            GameManager.Instance.PlayerDying();
        }
    }

    public void Celebrate()
    {
        TryChangeState(PlayerState.Celebrating);
    }

    private bool TryChangeState(PlayerState state)
    {
        if (State == state && State != default)
            return false;

        if (!Enum.IsDefined(typeof(PlayerState), state))
            throw new ArgumentOutOfRangeException(nameof(state), state, null);

        StopPendingStateAndEffectChanges();

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
                TryRemoveEffect(stopPendingStateAndEffectChanges: false);
                break;
            case PlayerState.Celebrating:
                SetMoveableAndCollidable(false);
                TryRemoveEffect(stopPendingStateAndEffectChanges: false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        OnStateChanged?.Invoke(state);

        return true;
    }

    private bool TryChangeEffect(PlayerEffect effect, float duration)
    {
        if (State != PlayerState.Normal)
            return false;

        if (Effect == effect)
            return false;

        if (!Enum.IsDefined(typeof(PlayerEffect), effect))
            throw new ArgumentOutOfRangeException(nameof(effect), effect, null);

        StopPendingStateAndEffectChanges();

        OnEffectChanging?.Invoke(effect);

        Effect = effect;

        this.DoAfterSeconds(duration, () => TryRemoveEffect());

        OnEffectChanged?.Invoke(effect);

        return true;
    }

    private bool TryRemoveEffect(bool stopPendingStateAndEffectChanges = true)
    {
        if (Effect == null)
            return false;

        PlayerEffect? effect = null;

        if (stopPendingStateAndEffectChanges)
            StopPendingStateAndEffectChanges();

        OnEffectChanging?.Invoke(effect);

        Effect = effect;
        SetMoveableAndCollidable(true);

        OnEffectChanged?.Invoke(effect);

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

    private void StopPendingStateAndEffectChanges()
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
    Dying,
    Celebrating
}

public enum PlayerEffect
{
    Invincible,
    Greedy
}