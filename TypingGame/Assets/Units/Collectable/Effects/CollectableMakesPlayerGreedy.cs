using UnityEngine;

public class CollectableMakesPlayerGreedy : CollectableStatusEffect
{
    [SerializeField] private float _durationSeconds;

    public override float DurationSeconds => _durationSeconds;

    public override void Trigger()
    {
        CollectableEffectManager.Instance.ApplyStatus<CollectableMakesPlayerGreedy>(this);
    }

    public override void ManagerApplyEffect()
    {
        SoundManager.Instance.PlayStatusEffectPositive();
        Player.Instance.BecomeGreedy();
        UnitManager.Instance.FrightenEnemies();
    }

    public override void ManagerRevertEffect()
    {
        Player.Instance.BecomeNotGreedy();
        UnitManager.Instance.EmboldenEnemies();
    }
}