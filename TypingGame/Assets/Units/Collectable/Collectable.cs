using System.Collections.Generic;
using UnityEngine;

public class Collectable : Unit
{
    private CollectableEffect[] _effects;

    private void Awake()
    {
        _effects = GetComponents<CollectableEffect>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out var player))
        {
            foreach(var effect in _effects) 
            {
                effect.RunCollectionEffect();
            }
            BeDestroyed();
        }
    }
}