using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected UnitMovement _movement;
    [SerializeField] protected UnitExploder _exploder;
    [SerializeField] protected float _spawningSeconds;

    protected UnitRespawner _optionalRespawner;

    public event Action<UnitState> OnStateChanging;
    public event Action<UnitState> OnStateChanged;
    public UnitState State { get; protected set; }

    protected virtual void Awake()
    {
        _optionalRespawner = GetComponent<UnitRespawner>();
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

    protected virtual bool TryChangeState(UnitState state)
    {
        if (State == state && State != default)
            return false;

        if (!Enum.IsDefined(typeof(UnitState), state))
            throw new ArgumentOutOfRangeException(nameof(state), state, null);

        OnStateChanging?.Invoke(state);

        StopPendingStateChanges();

        State = state;
        switch (state)
        {
            case UnitState.Spawning:
                SetMoveableAndCollidable(false);
                this.DoAfterSeconds(_spawningSeconds, () => TryChangeState(UnitState.Normal));
                break;
            case UnitState.Normal:
                SetMoveableAndCollidable(true);
                break;
            case UnitState.Destroyed:
                SetMoveableAndCollidable(false);
                _optionalRespawner?.RespawnLater();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

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
    Destroyed
}