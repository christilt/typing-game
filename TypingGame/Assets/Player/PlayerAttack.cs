using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float _collisionDuration;
    [SerializeField] private float _totalDuration;
    [SerializeField] private PlayerAttackVisual _visual;
    [SerializeField] private Collider2D _collider;

    private void Start()
    {
        PlayingVCamera.Instance.Camera.VeryQuickShake();

        _visual.AnimateAttack(_totalDuration);

        this.DoAfterSeconds(_collisionDuration, () =>
        {
            _collider.enabled = false;
        });

        this.DoAfterSeconds(_totalDuration, () =>
        {
            GameObject.Destroy(this.gameObject);
        });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Enemy>(out var enemy))
        {
            enemy.BeDestroyed();
        }
    }
}