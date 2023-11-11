using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected AiMovement _aiMovement;
    [SerializeField] protected float _spawningSeconds;
    [SerializeField] protected float _timeoutSeconds;
    [SerializeField] protected UnitExploder _exploder;

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
        if (State == UnitState.Destroyed)
            return;

        TryChangeState(UnitState.Destroyed);
        _exploder.Explode();
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
                //gameObject.SetActive(true);
                SetComponentsEnabled(false);
                this.DoAfterSeconds(_spawningSeconds, () => TryChangeState(UnitState.Normal));
                break;
            case UnitState.Normal:
                //gameObject.SetActive(true);
                SetComponentsEnabled(true);
                break;
            case UnitState.Destroyed:
                SetComponentsEnabled(false);
                //gameObject.SetActive(false);
                this.DoAfterSeconds(_spawningSeconds, () => Respawn());
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

    protected virtual void SetComponentsEnabled(bool enabled)
    {
        _aiMovement.enabled = enabled;
        _collider.enabled = enabled;
    }
}
public enum UnitState
{
    Spawning,
    Normal,
    Destroyed
}