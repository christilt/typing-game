using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class UnitExplosionPart : MonoBehaviour
{
    [SerializeField] private int _velocity;
    [SerializeField] private float _distance;

    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _startPosition;
    private float _currentDistance;

    public event Action<UnitExplosionPart> OnDistanceReached;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _startPosition = transform.position;
        _currentDistance = 0;
        _rigidBody.velocity = transform.up * _velocity;
    }

    private void FixedUpdate()
    {
        _currentDistance = Vector2.Distance(transform.position, _startPosition);
        var color = _spriteRenderer.color;
        color.a =  1f - (_currentDistance / _distance);
        _spriteRenderer.color = color;
        if (_currentDistance > _distance)
        {
            OnDistanceReached?.Invoke(this);
        }
    }
}