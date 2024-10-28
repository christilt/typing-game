public class CollectableKillsEnemies : CollectableEffect
{
    public override void ManagerApplyEffect()
    {
        SoundManager.Instance.PlayStatusEffectKillsEnemies();
        UnitManager.Instance.KillEnemies();
        PlayingVCamera.Instance.Camera.VeryQuickShake();
    }
}