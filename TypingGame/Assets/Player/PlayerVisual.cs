using System;
using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _celebrationParticles;
    [SerializeField] private SpriteRenderer _pacmanSprite;
    [SerializeField] private Player _player;

    [SerializeField] private float _celebrationSpinSpeed;

    public event Action OnPacmanExploding;
    public event Action OnPacmanExploded;

    private void Start()
    {
        _player.OnStateChanging += HandlePlayerStateChanging;
        _player.OnEffectChanging += HandlePlayerEffectChanging;
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnStateChanging -= HandlePlayerStateChanging;
            _player.OnEffectChanging -= HandlePlayerEffectChanging;
        }
    }

    private void HandlePlayerStateChanging(PlayerState state)
    {
        StopAnimations();

        switch (state)
        {
            case PlayerState.Initial:
                break;
            case PlayerState.Normal:
                break;
            case PlayerState.Dying:
                PacmanDie();
                break;
            case PlayerState.Celebrating:
                // TODO: Sometimes making player invisible because animation stopping and being disabled before returning to normal
                PacmanCelebrate();
                break;
            default:
                break;
        }
    }

    private void HandlePlayerEffectChanging(PlayerEffect? effect)
    {
        StopAnimations();

        if (effect == null)
            return;

        switch (effect)
        {
            case PlayerEffect.Invincible:
                PacmanInvincible();
                break;
            default:
                break;
        }
    }

    private void PacmanInvincible()
    {
        _animator.Play("Pacman_Invincible", -1, 0);
    }

    public void PacmanDie()
    {
        _animator.Play("Pacman_Die", -1, 0);
    }

    public void PacmanCelebrate()
    {
        _celebrationParticles.Play();
        _animator.enabled = false; // Otherwise rotation is prevented...

        StartCoroutine(SpinCoroutine());

        IEnumerator SpinCoroutine()
        {
            const int SPIN_DIRECTION = -1;
            // TODO - would this ever need cancelling?
            while (true)
            {
                var delta = Time.unscaledDeltaTime;
                var angle = delta * _celebrationSpinSpeed * SPIN_DIRECTION;
                _pacmanSprite.transform.eulerAngles += new Vector3(0, 0, angle);
                yield return null;
            }
        }
    }

    private void StopAnimations(string initialStateName = "Initial")
    {
        // TODO: Move to utility function?
        _animator.StopPlayback();
        _animator.Play(initialStateName, -1, 0);
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
