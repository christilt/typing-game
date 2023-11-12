using UnityEngine;

public abstract class CollectableEffect : MonoBehaviour
{
    public virtual void StartApplication()
    {
        CollectableEffectManager.Instance.Apply(this);
    }
    public abstract void ApplyCollectableEffect();
}