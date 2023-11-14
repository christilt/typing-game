using UnityEngine;

public class CollectableChangesUnitSpeed : CollectableStatusEffect
{
    [SerializeField, Range(0, 3)] private float _multiplier;
    [SerializeField] private float _durationSeconds;

    public override float DurationSeconds => _durationSeconds;

    // TODO: Maybe remove need for these with generics?
    public override void Trigger()
    {
        CollectableEffectManager.Instance.ApplyStatus<CollectableChangesUnitSpeed>(this);
    }

    public override void ManagerApplyEffect()
    {
        UnitManager.Instance.ChangeUnitSpeed(_multiplier);
    }

    public override void ManagerRevertEffect()
    {
        UnitManager.Instance.ResetUnitSpeed();
    }
}