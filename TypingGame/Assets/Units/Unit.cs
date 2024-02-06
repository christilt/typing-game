using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(UnitMovement), typeof(UnitBrain))]
public class Unit : MonoBehaviour
{
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected UnitExploder _exploder;
    [SerializeField] protected float _spawningSeconds;
    [SerializeField] protected Transform _centre;
    [SerializeField] protected bool _beginDestroyed;
    [SerializeField] protected LayerMask _excludeCollisionLayersWhenSpawning; 

    protected UnitMovement _movement;
    protected UnitBrain _brain;

    protected UnitBoundary _optionalBoundary;
    protected UnitRespawner _optionalRespawner;

    public event Action<UnitState> OnStateChanging;
    public event Action<UnitState> OnStateChanged;
    public UnitState State { get; protected set; }
    public Vector2 Centre => _centre.position;
    public PositionWithCentre PositionWithCentre => new PositionWithCentre(transform.position, Centre);
    public bool HasBoundary => _optionalBoundary != null;

    protected virtual void Awake()
    {
        _optionalBoundary = GetComponent<UnitBoundary>();   
        _optionalRespawner = GetComponent<UnitRespawner>();
        _movement = GetComponent<UnitMovement>();
        _brain = GetComponent<UnitBrain>();
    }

    protected virtual void Start()
    {
        var startState = _beginDestroyed ? UnitState.Destroyed : UnitState.Spawning;
        TryChangeState(startState);
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

    public virtual void ChangeSpeed(float multiplier)
    {
        _movement.SpeedMultiplier = multiplier;
    }

    public virtual void ResetSpeed()
    {
        _movement.SpeedMultiplier = 1;
    }

    public virtual void FearPlayer() => TryChangeState(UnitState.FearPlayer);

    public virtual void DoNotFearPlayer() => BecomeNormalIfStatus(UnitState.FearPlayer);

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
                DisableMovementAndNonSpawningCollisions();
                _brain.SetInitialMode();
                this.DoAfterSeconds(_spawningSeconds, () => TryChangeState(UnitState.Normal));
                break;
            case UnitState.Normal:
                EnableMovementAndCollisions();
                _brain.SetInitialMode();
                break;
            case UnitState.FearPlayer:
                EnableMovementAndCollisions();
                _brain.MaybeEvadePlayer();
                break;
            case UnitState.Destroyed:
                DisableMovementAndAllCollisions();
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

    private void BecomeNormalIfStatus(UnitState status)
    {
        if (State != status)
            return;

        TryChangeState(UnitState.Normal);
    }

    protected virtual void StopPendingStateChanges()
    {
        StopAllCoroutines();
        _optionalRespawner?.Stop();
    }

    protected virtual void EnableMovementAndCollisions()
    {
        _movement.enabled = true;
        _collider.enabled = true;
        _collider.excludeLayers = 0;
    }

    protected virtual void DisableMovementAndAllCollisions()
    {
        _movement.enabled = false;
        _collider.enabled = false;
        _collider.excludeLayers = 0;
    }

    protected virtual void DisableMovementAndNonSpawningCollisions()
    {
        _movement.enabled = false;
        _collider.enabled = true;
        _collider.excludeLayers = _excludeCollisionLayersWhenSpawning; 
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