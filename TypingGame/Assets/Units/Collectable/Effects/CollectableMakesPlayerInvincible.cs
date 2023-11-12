using UnityEngine;

public class CollectableMakesPlayerInvincible : CollectableEffect
{
    [SerializeField] private float _durationSeconds;

    public override void RunCollectionEffect()
    {
        Player.Instance.BecomeInvincible(_durationSeconds);
    }
}