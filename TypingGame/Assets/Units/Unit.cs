﻿using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected UnitMovement _movement;
    [SerializeField] protected UnitExploder _exploder;
    [SerializeField] protected float _spawningSeconds;
    [SerializeField] protected float _destroyedSeconds;

    protected Vector3 _startPosition;

    public event Action<UnitState> OnStateChanging;
    public event Action<UnitState> OnStateChanged;
    public UnitState State { get; protected set; }

    protected virtual void Start()
    {
        _startPosition = transform.position;
        TryChangeState(UnitState.Spawning);
    }

    public virtual void BeDestroyed()
    {
        if (TryChangeState(UnitState.Destroyed))
        {
            _exploder.Explode();
        }
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

        State = state;
        switch (state)
        {
            case UnitState.Spawning:
                SetComponentsEnabled(false);
                this.DoAfterSeconds(_spawningSeconds, () => TryChangeState(UnitState.Normal));
                break;
            case UnitState.Normal:
                SetComponentsEnabled(true);
                break;
            case UnitState.Destroyed:
                SetComponentsEnabled(false);
                this.DoAfterSeconds(_destroyedSeconds, () => Respawn());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        OnStateChanged?.Invoke(state);

        return true;
    }

    protected virtual void Respawn()
    {
        transform.position = _startPosition;
        TryChangeState(UnitState.Spawning);
    }

    protected virtual void ResetSpeed()
    {
        _movement.SpeedMultiplier = 1;
    }

    protected virtual void SetComponentsEnabled(bool enabled)
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