using UnityEngine;

public static class CollisionExtensions
{
    public static bool TryGetRigidbodyComponent<T>(this Collision2D collision, out T component)
    {
        return collision.gameObject.TryGetComponent(out component);
    }
    public static bool TryGetRigidbodyComponent<T>(this Collider2D collider, out T component)
    {
        return collider.attachedRigidbody.gameObject.TryGetComponent(out component);
    }
}