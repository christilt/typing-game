public class CollectableKillsEnemies : CollectableEffect
{
    public override void ApplyCollectableEffect()
    {
        UnitManager.Instance.KillEnemies();
    }
}