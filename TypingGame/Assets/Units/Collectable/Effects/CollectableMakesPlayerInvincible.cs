using UnityEngine;

public class CollectableMakesPlayerInvincible : CollectableStatusEffect
{
    [SerializeField] private float _durationSeconds;

    public override float DurationSeconds => _durationSeconds;

    public override void Trigger()
    {
        CollectableEffectManager.Instance.ApplyStatus<CollectableMakesPlayerInvincible>(this);
    }

    public override void ManagerApplyEffect()
    {
        SoundManager.Instance.PlayStatusEffectPositive();
        Player.Instance.BecomeInvincible();
    }

    public override void ManagerRevertEffect()
    {
        Player.Instance.BecomeNotInvincible();
    }
}