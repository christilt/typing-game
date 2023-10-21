using System;
using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _celebrationParticles;
    [SerializeField] private SpriteRenderer _pacmanSprite;

    [SerializeField] private float _celebrationSpinSpeed;

    public event Action OnPacmanExploding;
    public event Action OnPacmanExploded;

    public void PacmanCelebrate()
    {
        _celebrationParticles.Play();
        // TODO was working, now not working.  Maybe to do with rigidbody?  https://forum.unity.com/threads/transform-rotation-not-working.1168475/
        StartCoroutine(SpinCoroutine());

        IEnumerator SpinCoroutine()
        {
            const int SPIN_DIRECTION = -1;
            // TODO - would this ever need cancelling?
            while (true)
            {
                var delta = Time.unscaledDeltaTime;
                var angle = delta * _celebrationSpinSpeed * SPIN_DIRECTION;
                _pacmanSprite.transform.Rotate(new Vector3(0, 0, angle));
                yield return null;
            }
        }
    }

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
