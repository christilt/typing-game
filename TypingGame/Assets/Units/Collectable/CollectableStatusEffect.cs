using UnityEngine;

public abstract class CollectableStatusEffect : CollectableEffect
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Sprite _sprite;

    private void Awake()
    {
        _sprite = _spriteRenderer.sprite;    
    }

    public abstract float DurationSeconds { get; }

    public Sprite Sprite => _sprite;

    public abstract void ManagerRevertEffect();

}