public class CollectableGoal : Collectable, ICollectableGoal
{
    protected virtual void Awake()
    {
        Id = transform.GetInstanceID();
    }

    protected override void Start()
    {
        GoalManager.Instance.TryRegister(this);
        base.Start();
    }

    public int Id { get; protected set; }

    public override void BeCollected()
    {
        GoalManager.Instance.Complete(this);
        BeDestroyed();
    }

}