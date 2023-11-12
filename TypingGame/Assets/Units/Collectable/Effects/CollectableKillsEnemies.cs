public class CollectableKillsEnemies : CollectableEffect
{
    public override void RunCollectionEffect()
    {
        UnitManager.Instance.KillEnemies();
    }
}