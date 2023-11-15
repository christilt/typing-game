public class CollectableKillsEnemies : CollectableEffect
{
    public override void ManagerApplyEffect()
    {
        UnitManager.Instance.KillEnemies();
        PlayingVCamera.Instance.Camera.VeryQuickShake();
    }
}