using UnityEngine;

[RequireComponent (typeof(Hider))]
public class LevelHider : Singleton<LevelHider>
{
    private Hider _hider;

    protected override void Awake()
    {
        base.Awake();

        _hider = GetComponent<Hider>();
    }

    public Hider Hider { get => _hider; }
}