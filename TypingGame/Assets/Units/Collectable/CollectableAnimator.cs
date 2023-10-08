using UnityEngine;

public class CollectableAnimator : MonoBehaviour
{
    [SerializeField] private AiMovement _movement;
    [SerializeField] private Animator _animator;

    private void Update()
    {
        _animator.SetBool("IsMoving", _movement.Direction != default);
    }
}