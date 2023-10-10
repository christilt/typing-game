using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private AiMovement _aiMovement;
    [SerializeField] private float _spawningSeconds;

    public event Action<EnemyState> OnStateChanging;
    public event Action<EnemyState> OnStateChanged;
    public EnemyState State { get; private set; }

    private void Start()
    {
        TryChangeState(EnemyState.Spawning);
    }

    // TODO could be OTT
    private bool TryChangeState(EnemyState state)
    {
        if (State == state && State != default)
            return false;

        if (!Enum.IsDefined(typeof(EnemyState), state))
            throw new ArgumentOutOfRangeException(nameof(state), state, null);

        OnStateChanging?.Invoke(state);

        State = state;
        switch (state)
        {
            case EnemyState.Spawning:
                _aiMovement.enabled = false;
                _collider.enabled = false;
                this.DoAfterSeconds(_spawningSeconds, () => TryChangeState(EnemyState.Normal));
                break;
            case EnemyState.Normal:
                _aiMovement.enabled = true;
                _collider.enabled = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        OnStateChanged?.Invoke(state);

        return true;

    }
}
public enum EnemyState
{
    Spawning,
    Normal
}