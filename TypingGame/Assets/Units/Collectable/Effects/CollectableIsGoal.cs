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
    public bool IsCompleted { get; private set; } 

    public override void ManagerApplyEffect()
    {
        IsCompleted = true;
        SoundManager.Instance.PlayCollectGoal();
        GoalManager.Instance.Complete(this);
    }
}