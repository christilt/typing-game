using UnityEngine;

public class CollectableMakesPlayerGreedy : CollectableStatusEffect
{
    [SerializeField] private float _durationSeconds;

    public override float DurationSeconds => _durationSeconds;

    public override void StartApplication()
    {
        CollectableEffectManager.Instance.ApplyStatus<CollectableMakesPlayerGreedy>(this);
    }

    public override void ApplyCollectableEffect()
    {
        Player.Instance.BecomeGreedy();
        UnitManager.Instance.FrightenEnemies();
    }

    public override void RevertCollectableEffect()
    {
        Player.Instance.BecomeNotGreedy();
        UnitManager.Instance.EmboldenEnemies();
    }
}