public class CollectableGoal : Collectable, ICollectableGoal
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

    public override void BeCollected()
    {
        GoalManager.Instance.Complete(this);
        DestroySelf();
    }

}