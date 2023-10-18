using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Singleton<Player>
{
    [SerializeField] private PlayerTypingMovement _typingMovement;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private GameObject _visual;

    public Transform VisualTransform => _visual.transform;

    private void Start()
    {
        GameManager.Instance.OnStateChanging += HandleGameStateChanging;
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
    }

    private void HandleGameStateChanging(GameState state)
    {
        if (state == GameState.LevelPlaying)
        {
            _typingMovement.EnableComponent();
            _collider.enabled = true;
        }
        else
        {
            _typingMovement.DisableComponent();
            _collider.enabled = false;
        }
    }
}
