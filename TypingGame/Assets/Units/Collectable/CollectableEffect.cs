using UnityEngine;

public abstract class CollectableEffect : MonoBehaviour
{
    public virtual void Trigger()
    {
        CollectableEffectManager.Instance.Apply(this);
    }
    public abstract void ManagerApplyEffect();
}