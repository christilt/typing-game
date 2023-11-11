using UnityEngine;

public class Collectable : Unit
{
    public virtual void BeCollected()
    {
        BeDestroyed();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out var player))
        {
            BeCollected();
        }
    }
}