using System;
using UnityEngine;

public class UnitExplosionPart : MonoBehaviour
{
    [SerializeField] private int _velocity;
    [SerializeField] private float _distance;

    private Vector2 _startPosition;
    private float _currentDistance;

    public SpriteRenderer SpriteRenderer { get; private set; }
    
    public event Action<UnitExplosionPart> OnDistanceReached;

    public static UnitExplosionPart InstantiateInPool(UnitExplosionPart prefab, Sprite prefabSprite)
    {
        prefab.gameObject.SetActive(false);
        var obj = Instantiate(prefab);
        var spriteRenderer = obj.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = prefabSprite;
        obj.SpriteRenderer = spriteRenderer;

        return obj;
    }

    private void OnEnable()
    {
        _startPosition = transform.position;
        _currentDistance = 0;
    }

    private void Update()
    {
        UpdateMovement();
        UpdateTransparency();
    }

    private void UpdateMovement()
    {
        transform.Translate(transform.right * _velocity * Time.deltaTime, Space.World);

        _currentDistance = Vector2.Distance(transform.position, _startPosition);
        if (_currentDistance > _distance)
        {
            OnDistanceReached?.Invoke(this);
        }
    }

    private void UpdateTransparency()
    {
        if (SpriteRenderer == null)
            return;

        var color = SpriteRenderer.color;
        color.a = 1f - (_currentDistance / _distance);
        SpriteRenderer.color = color;
    }
}