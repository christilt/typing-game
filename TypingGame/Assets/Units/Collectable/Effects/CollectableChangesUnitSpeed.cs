using UnityEngine;

public class CollectableChangesUnitSpeed : CollectableEffect
{
    [SerializeField, Range(0, 3)] private float multiplier;
    [SerializeField] private float durationSeconds;

    public override void RunCollectionEffect()
    {
        UnitManager.Instance.ChangeUnitSpeed(multiplier, durationSeconds);
    }
}