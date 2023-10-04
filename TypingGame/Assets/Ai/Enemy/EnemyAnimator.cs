using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private AiMovement _enemyMovement;

    [SerializeField] private Animator _animator;

    private void Update()
    {
        _animator.SetFloat("x", _enemyMovement.Direction.x);
        _animator.SetFloat("y", _enemyMovement.Direction.y);
    }
}
