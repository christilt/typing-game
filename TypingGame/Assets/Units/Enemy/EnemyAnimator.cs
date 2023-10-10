using UnityEngine;

// TODO unsubscribe any events ondestroy
public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private AiMovement _enemyMovement;

    [SerializeField] private Animator _animator;

    private void Start()
    {
        SetAnimatorIsInvincible(_enemy.State);
        _enemy.OnStateChanging += OnEnemyStateChanging;
    }

    private void Update()
    {
        // TODO check velocity also
        _animator.SetFloat("X", _enemyMovement.Direction.x);
        _animator.SetFloat("Y", _enemyMovement.Direction.y);
    }
    private void OnDestroy()
    {
        _enemy.OnStateChanging -= OnEnemyStateChanging;
    }

    private void OnEnemyStateChanging(EnemyState state)
    {
        SetAnimatorIsInvincible(state);
    }

    private void SetAnimatorIsInvincible(EnemyState state)
    {
        _animator.SetBool("IsInvincible", state == EnemyState.Spawning);
    }
}
