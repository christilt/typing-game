using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(UnitMovement), typeof(UnitBrain))]
public class Unit : MonoBehaviour
{
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected UnitExploder _exploder;
    [SerializeField] protected float _spawningSeconds;
    [SerializeField] protected Transform _centre;

    protected UnitMovement _movement;
    protected UnitBrain _brain;

    protected UnitRespawner _optionalRespawner;

    public event Action<UnitState> OnStateChanging;
    public event Action<UnitState> OnStateChanged;
    public UnitState State { get; protected set; }
    public Vector2 Centre => _centre.position;
    public PositionWithCentre PositionWithCentre => new PositionWithCentre(transform.position, Centre);

    protected virtual void Awake()
    {
        _optionalRespawner = GetComponent<UnitRespawner>();
        _movement = GetComponent<UnitMovement>();
        _brain = GetComponent<UnitBrain>();
    }

    protected virtual void Start()
    {
        TryChangeState(UnitState.Spawning);
    }

    public virtual void BeDestroyed()
    {
        if (TryChangeState(UnitState.Destroyed))
        {
            _exploder.Explode();
        }
    }

    public virtual void BeSpawned()
    {
        TryChangeState(UnitState.Spawning);
    }

    public virtual void ChangeSpeed(float multiplier, float durationSeconds)
    {
        CancelInvoke(nameof(ResetSpeed));
        _movement.SpeedMultiplier = multiplier;
        Invoke(nameof(ResetSpeed), durationSeconds); // Use invoke so not interrupted by changing state
    }

    public virtual void FearPlayer(float durationSeconds)
    {
        TryChangeState(UnitState.FearPlayer, durationSeconds);
    }

    protected virtual bool TryChangeState(UnitState state, float? revertAfterSeconds = null)
    {
        if (State == state && State != default)
            return false;

        if (!Enum.IsDefined(typeof(UnitState), state))
            throw new ArgumentOutOfRangeException(nameof(state), state, null);

        if (revertAfterSeconds.HasValue && !state.IsRevertable())
            throw new ArgumentException($"Cannot specify {nameof(revertAfterSeconds)} with state {state} as not revertable");

        StopPendingStateChanges();

        OnStateChanging?.Invoke(state);

        State = state;
        switch (state)
        {
            case UnitState.Spawning:
                SetMoveableAndCollidable(false);
                _brain.SetInitialMode();
                this.DoAfterSeconds(_spawningSeconds, () => TryChangeState(UnitState.Normal));
                break;
            case UnitState.Normal:
                SetMoveableAndCollidable(true);
                _brain.SetInitialMode();
                break;
            case UnitState.FearPlayer:
                SetMoveableAndCollidable(true);
                _brain.MaybeEvadePlayer();
                break;
            case UnitState.Destroyed:
                SetMoveableAndCollidable(false);
                _optionalRespawner?.RespawnLater();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        if (revertAfterSeconds.HasValue)
            this.DoAfterSeconds(revertAfterSeconds.Value, () => TryChangeState(UnitState.Normal));

        OnStateChanged?.Invoke(state);

        return true;
    }

    protected virtual void ResetSpeed()
    {
        _movement.SpeedMultiplier = 1;
    }

    protected virtual void StopPendingStateChanges()
    {
        StopAllCoroutines();
        _optionalRespawner?.Stop();
    }

    protected virtual void SetMoveableAndCollidable(bool enabled)
    {
        _movement.enabled = enabled;
        _collider.enabled = enabled;
    }
}
public enum UnitState
{
    Spawning,
    Normal,
    FearPlayer,
    Destroyed
}

public static class UnitStateExtensions
{
    public static bool IsRevertable(this UnitState state)
    {
        return state == UnitState.FearPlayer;
    }
}