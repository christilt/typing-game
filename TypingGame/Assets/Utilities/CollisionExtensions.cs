using UnityEngine;

public static class CollisionExtensions
{
    public static bool TryGetRigidbodyComponent<T>(this Collision2D collision, out T component)
    {
        component = default;

        if (collision.gameObject == null)
            return false;

        return collision.gameObject.TryGetComponent(out component);
    }
    public static bool TryGetRigidbodyComponent<T>(this Collider2D collider, out T component)
    {
        component = default;

        if (collider.attachedRigidbody?.gameObject == null)
            return false;

        return collider.attachedRigidbody.gameObject.TryGetComponent(out component);
    }
}