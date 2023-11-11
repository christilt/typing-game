public class CollectableEffect_KillEnemies : Collectable
{
    public override void BeCollected()
    {
        UnitManager.Instance.KillAllEnemies();
        BeDestroyed();
    }

}