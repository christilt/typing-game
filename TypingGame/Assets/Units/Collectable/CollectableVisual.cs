using UnityEngine;

public class CollectableVisual : UnitVisual
{
    [SerializeField] private AiMovement _movement;

    protected void FixedUpdate()
    {
        _animator.SetBool("IsMoving", _movement.Direction != default);
    }
}