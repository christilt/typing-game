﻿using System.Collections.Generic;
using UnityEngine;

public class Collectable : Unit
{
    protected CollectableEffect[] _effects;

    protected override void Awake()
    {
        base.Awake();
        _effects = GetComponents<CollectableEffect>();
    }

    protected override void Start()
    {
        base.Start();
        UnitManager.Instance.TryRegister(this);
    }

    public CollectableEffect[] Effects => _effects;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out var player))
        {
            player.HitCollectable(this);
        }
    }

    public void BeEffected()
    {
        foreach (var effect in _effects)
        {
            effect.Trigger();
        }
        BeDestroyed();
    }
}