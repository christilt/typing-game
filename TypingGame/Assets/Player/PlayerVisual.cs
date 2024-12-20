﻿using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _celebrationParticles;
    [SerializeField] private SpriteRenderer _pacmanSprite;
    [SerializeField] private Player _player;
    [SerializeField] private Light2D _light;

    [SerializeField] private float _celebrationSpinSpeed;
    [SerializeField] private float _greedyLightIntensity;

    private float _lightInitialIntensity;

    public event Action OnPacmanExploding;
    public event Action OnPacmanExploded;

    private void Start()
    {
        _lightInitialIntensity = _light.intensity;
        _player.OnStateChanging += HandlePlayerStateChanging;
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnStateChanging -= HandlePlayerStateChanging;
        }
    }

    private void HandlePlayerStateChanging(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Initial:
                PacmanIdle();
                break;
            case PlayerState.Normal:
                PacmanIdle();
                break;
            case PlayerState.Dying:
                PacmanDie();
                break;
            case PlayerState.Celebrating:
                PacmanCelebrate();
                break;
            case PlayerState.Invincible:
                PacmanInvincible();
                break;
            case PlayerState.Greedy:
                PacmanGreedy();
                break;
            default:
                break;
        }
    }

    private void PacmanIdle()
    {
        _light.intensity = _lightInitialIntensity;
        _animator.Play("Initial", -1, 0);
    }

    private void PacmanInvincible()
    {
        _animator.Play("Pacman_Invincible", -1, 0);
    }

    private void PacmanGreedy()
    {
        _light.intensity = _greedyLightIntensity;
        _animator.Play("Initial", -1, 0);
    }

    private void PacmanDie()
    {
        _light.intensity = _lightInitialIntensity;
        _animator.Play("Pacman_Die", -1, 0);
    }

    private void PacmanCelebrate()
    {
        _light.intensity = _lightInitialIntensity;
        _celebrationParticles.Play();
        _animator.Play("Pacman_Celebrate", -1, 0);
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
