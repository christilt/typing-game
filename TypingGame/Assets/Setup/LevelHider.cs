using UnityEngine;

public class LevelHider : Singleton<LevelHider>
{
    [SerializeField] private Hider _hider;

    public Hider Hider { get => _hider; }
}