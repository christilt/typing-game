using UnityEngine;

public class CollectableVisual : UnitVisual
{
    [SerializeField] private UnitMovement _movement;

    protected void FixedUpdate()
    {
        _animator.SetBool("IsMoving", _movement.Direction != default);
    }
}