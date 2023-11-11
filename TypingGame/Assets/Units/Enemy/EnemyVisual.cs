using UnityEngine;

// TODO unsubscribe any events ondestroy
public class EnemyVisual : UnitVisual
{
    [SerializeField] private AiMovement _enemyMovement;

    private void Update()
    {
        _animator.SetFloat("X", _enemyMovement.Direction.x);
        _animator.SetFloat("Y", _enemyMovement.Direction.y);
    }
}
