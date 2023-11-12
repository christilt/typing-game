using UnityEngine;

public abstract class CollectableStatusEffect : CollectableEffect
{
    public abstract float DurationSeconds { get; }
    public abstract void RevertCollectableEffect();

}