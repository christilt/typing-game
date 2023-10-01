using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCollisions : MonoBehaviour
{
    public event EventHandler<CollectableCollectedEventArgs> OnCollectableCollected;
    public class CollectableCollectedEventArgs : EventArgs { public string Name; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO
        Debug.Log($"Player Hit {collision.gameObject.name}");

        if (collision.gameObject.TryGetComponent<CollectableCollisions>(out var collectable))
        {
            collectable.DestroySelf();
            OnCollectableCollected?.Invoke(this, new CollectableCollectedEventArgs { Name = collectable.name });
        }
    }
}
