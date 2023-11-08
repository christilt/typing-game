using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnitExplosionPart : MonoBehaviour
{
    [SerializeField] private int _velocity;
    [SerializeField] private float _distance;

    private Rigidbody2D _rigidBody;
    private Vector2 _startPosition;
    private float _currentDistance;

    public SpriteRenderer SpriteRenderer { get; private set; }
    
    public event Action<UnitExplosionPart> OnDistanceReached;

    public static UnitExplosionPart Instantiate(UnitExplosionPart prefab, Vector3 position, Quaternion euler, Transform parent, Sprite prefabSprite)
    {
        prefab.gameObject.SetActive(false);
        var obj = Instantiate(prefab, position, euler, parent);
        var spriteRenderer = obj.gameObject.AddComponent<SpriteRenderer>();
        // TODO: PPtr cast failed when dereferencing! Casting from SpriteRenderer to Sprite
        spriteRenderer.sprite = prefabSprite;
        obj.SpriteRenderer = spriteRenderer;
        obj.gameObject.SetActive(true);
        return obj;
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
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
        if (_currentDistance > _distance)
        {
            OnDistanceReached?.Invoke(this);
        }

        if (SpriteRenderer == null)
            return;

        var color = SpriteRenderer.color;
        color.a =  1f - (_currentDistance / _distance);
        SpriteRenderer.color = color;
    }
}