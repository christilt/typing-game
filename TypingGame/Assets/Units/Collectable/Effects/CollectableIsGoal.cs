using UnityEngine;

public class CollectableIsGoal : CollectableEffect, ICollectableGoal
{
    protected virtual void Awake()
    {
        Id = transform.GetInstanceID();
    }

    protected virtual void Start()
    {
        GoalManager.Instance.TryRegister(this);
    }

    public int Id { get; protected set; }

    public override void ManagerApplyEffect()
    {
        GoalManager.Instance.Complete(this);
    }
}