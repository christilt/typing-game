using UnityEngine;

public class CollectableMakesPlayerGreedy : CollectableEffect
{
    [SerializeField] private float _durationSeconds;

    public override void RunCollectionEffect()
    {
        Player.Instance.BecomeGreedy(_durationSeconds);
        UnitManager.Instance.FrightenEnemies(_durationSeconds);
    }
}