﻿using UnityEngine;

// TODO: Unused: Remove?
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] private Animator _animator;

    private void Update()
    {
        _animator.SetFloat("x", _playerMovement.Direction.x);
        _animator.SetFloat("y", _playerMovement.Direction.y);
    }
}
