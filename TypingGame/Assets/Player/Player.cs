using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerTypingMovement _typingMovement;
    [SerializeField] private Collider2D _collider;

    private void Start()
    {
        GameManager.Instance.OnStateChanging += HandleGameStateChanging;
        PauseManager.Instance.OnPauseChanging += HandlePauseStateChanging;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Collectable>(out var collectable))
        {
            collectable.DestroySelf();
        }

        if (collision.TryGetRigidbodyComponent<Enemy>(out var enemy))
        {
            GameManager.Instance.PlayerDying();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanging -= HandleGameStateChanging;
        if (PauseManager.Instance != null)
            PauseManager.Instance.OnPauseChanging -= HandlePauseStateChanging;
    }

    private void HandleGameStateChanging(GameState state)
    {
        if (state == GameState.LevelPlaying)
            _collider.enabled = true;
        else
            _collider.enabled = false;
    }

    private void HandlePauseStateChanging(PauseState pauseState)
    {
        if (pauseState == PauseState.Unpaused)
            _typingMovement.EnableComponent();
        else
            _typingMovement.DisableComponent();
    }
}
