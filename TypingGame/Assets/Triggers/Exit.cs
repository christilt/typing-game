using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetRigidbodyComponent(out Player player))
        {
            // TODO - make this a goal?
            GameManager.Instance.LevelWinning();
        }
    }
}