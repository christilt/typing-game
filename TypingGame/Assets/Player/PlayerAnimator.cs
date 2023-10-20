using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public event Action OnPacmanExploding;
    public event Action OnPacmanExploded;

    public void PacmanDie()
    {
        _animator.Play("Pacman_Die", -1, 0);
    }

    public void OnAnimationPacmanExploding()
    {
        OnPacmanExploding?.Invoke();
    }

    public void OnAnimationPacmanExploded()
    {
        OnPacmanExploded?.Invoke();
    }
}
