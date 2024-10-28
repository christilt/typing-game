using Cinemachine;
using System;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
public class Player : Singleton<Player>
{
    [SerializeField] private PlayerVisual _visual;
    [SerializeField] private PlayerTypingMovement _typingMovement;
    [SerializeField] private PlayerInputController _inputController;
    [SerializeField] private Collider2D _collider;

    private void Start()
    {
        GameplayManager.Instance.OnStateChanging += HandleGameStateChanging;
        _visual.OnPacmanExploding += HandlePacmanExploding;
        _visual.OnPacmanExploded += HandlePacmanExploded;
        _typingMovement.OnCorrectKeyTyped += HandleCorrectKeyTyped;
        _typingMovement.OnIncorrectKeyTyped += HandleIncorrectKeyTyped;
        _inputController.OnAttack += HandleAttack;
        _inputController.OnPauseOrUnpause += HandlePauseOrUnpause;
    }

    private void OnDestroy()
    {
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnStateChanging -= HandleGameStateChanging;
        }
        if (_visual != null)
        {
            _visual.OnPacmanExploding -= HandlePacmanExploding;
            _visual.OnPacmanExploded -= HandlePacmanExploded;
        }
        if (_typingMovement != null)
        {
            _typingMovement.OnCorrectKeyTyped -= HandleCorrectKeyTyped;
            _typingMovement.OnIncorrectKeyTyped -= HandleIncorrectKeyTyped;
        }
        if (_inputController != null)
        {
            _inputController.OnAttack -= HandleAttack;
            _inputController.OnPauseOrUnpause -= HandlePauseOrUnpause;
        }
    }

    private void HandleAttack()
    {
        SoundManager.Instance.PlayPlayerAttack();
    }

    private void HandlePauseOrUnpause()
    {

    }

    private void HandleCorrectKeyTyped(KeyTile keyTile)
    {
        SoundManager.Instance.PlayTypeHit();
        StatsManager.Instance.TypingRecorder.LogCorrectKey(keyTile);
    }
    private void HandleIncorrectKeyTyped(KeyTile keyTile)
    {
        SoundManager.Instance.PlayTypeMiss();
        StatsManager.Instance.TypingRecorder.LogIncorrectKey();
    }

    public event Action<PlayerState> OnStateChanging;
    public event Action<PlayerState> OnStateChanged;

    public PlayerState State { get; private set; }

    public Vector2 Centre => _visual.transform.position;

    public void SetAsFollow(CinemachineVirtualCamera camera) => camera.Follow = _visual.transform;

    public void BecomeInvincible() => TryChangeState(PlayerState.Invincible);

    public void BecomeNotInvincible() => BecomeNormalIfStatus(PlayerState.Invincible);

    public void BecomeGreedy() => TryChangeState(PlayerState.Greedy);

    public void BecomeNotGreedy() => BecomeNormalIfStatus(PlayerState.Greedy);

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
            GameplayManager.Instance.PlayerDying();
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

        if (state.InterruptsPendingStateChanges())
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
            case PlayerState.Paused:
                SetMoveableAndCollidable(false);
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

    private void BecomeNormalIfStatus(PlayerState status)
    {
        if (State != status)
            return;

        TryChangeState(PlayerState.Normal);
    }

    private void HandleGameStateChanging(GameState state)
    {
        switch (state)
        {
            case GameState.LevelPlaying:
                TryChangeState(PlayerState.Normal);
                break;
            case GameState.LevelPausing:
                TryChangeState(PlayerState.Paused);
                break;
            case GameState.LevelWinning:
                Celebrate();
                break;
            default:
                break; // Do nothing for other state changes
        }

        var isPausable = state.AllowsPausing() || state.AllowsUnpausing();
        SetPausableAndUnpausable(isPausable);
    }

    private void HandlePacmanExploding()
    {
        GameplayManager.Instance.PlayerExploding();
    }

    private void HandlePacmanExploded()
    {
        GameplayManager.Instance.PlayerExploded();
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
        {
            _typingMovement.EnableComponent();
            _inputController.EnableAttack();
        }
        else
        {
            _typingMovement.DisableComponent();
            _inputController.DisableAttack();
        }
    }

    private void SetPausableAndUnpausable(bool enabled)
    {
        if (enabled)
        {
            _inputController.EnablePauseAndUnpause();
        }
        else
        {
            _inputController.DisablePauseAndUnpause();
        }
    }
}

public enum PlayerState
{
    Initial,
    Normal,
    Paused,
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

    public static bool InterruptsPendingStateChanges(this PlayerState state)
    {
        return state != PlayerState.Paused;
    }
}