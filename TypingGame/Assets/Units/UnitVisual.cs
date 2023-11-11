using UnityEngine;

public class UnitVisual : MonoBehaviour
{

    [SerializeField] protected Unit _unit;
    [SerializeField] protected SpriteRenderer _sprite;

    [SerializeField] protected Animator _animator;

    protected virtual void Start()
    {
        _unit.OnStateChanging += OnUnitStateChanging;
        OnUnitStateChanging(_unit.State);
    }

    protected virtual void OnDestroy()
    {
        if (_unit != null)
            _unit.OnStateChanging -= OnUnitStateChanging;
    }

    protected virtual void OnUnitStateChanging(UnitState state)
    {
        //SetTransparency(state);
        SetAnimation(state);
    }

    protected virtual void SetTransparency(UnitState state)
    {
        var color = _sprite.color;
        color.a = state == UnitState.Destroyed ? 0f : 1f;
        _sprite.color = color;
    }

    protected virtual void SetAnimation(UnitState state)
    {
        _animator.SetBool("IsInvincible", state == UnitState.Spawning);
    }
}
