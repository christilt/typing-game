using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Collectable : MonoBehaviour
{
    [SerializeField] private UnitExploder _exploder;

    public void DestroySelf()
    {
        _exploder.Explode();
    }

    public virtual void PlayerCollect()
    {
        DestroySelf();
    }
}