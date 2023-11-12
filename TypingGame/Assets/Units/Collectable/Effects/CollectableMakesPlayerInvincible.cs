using UnityEngine;

public class CollectableMakesPlayerInvincible : CollectableStatusEffect
{
    [SerializeField] private float _durationSeconds;

    public override float DurationSeconds => _durationSeconds;

    public override void StartApplication()
    {
        CollectableEffectManager.Instance.ApplyStatus<CollectableMakesPlayerInvincible>(this);
    }

    public override void ApplyCollectableEffect()
    {
        Player.Instance.BecomeInvincible();
    }

    public override void RevertCollectableEffect()
    {
        Player.Instance.BecomeNotInvincible();
    }
}