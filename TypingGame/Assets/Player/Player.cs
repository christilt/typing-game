using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerTypingMovement _typingMovement;

    public event EventHandler<CollectableCollectedEventArgs> OnCollectableCollected;
    public class CollectableCollectedEventArgs : EventArgs { public string Name; }
    private void Start()
    {
        PauseManager.Instance.OnPauseChanging += HandlePauseChanging;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO
        Debug.Log($"Player Hit {collision.gameObject.name}");

        if (collision.gameObject.TryGetComponent<Collectable>(out var collectable))
        {
            collectable.DestroySelf();
            OnCollectableCollected?.Invoke(this, new CollectableCollectedEventArgs { Name = collectable.name });
        }

        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            GameManager.Instance.PlayerDying();
        }
    }

    private void OnDestroy()
    {
        PauseManager.Instance.OnPauseChanging -= HandlePauseChanging;
    }

    private void HandlePauseChanging(bool paused)
    {
        if (paused)
            _typingMovement.DisableComponent();
        else
            _typingMovement.EnableComponent();
    }
}
