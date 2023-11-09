using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Collectable : MonoBehaviour
{
    [SerializeField] private UnitExploder _exploder;

    public void DestroySelf()
    {
        _exploder.Explode();
    }

    public virtual void BeCollected()
    {
        DestroySelf();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out var player))
        {
            BeCollected();
        }
    }
}