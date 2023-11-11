using System;
using UnityEngine;

public class Enemy : Unit
{
    protected override void Start()
    {
        base.Start();
        UnitManager.Instance.TryRegister(this);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out var player))
        {
            player.BeKilled();
        }
    }
}