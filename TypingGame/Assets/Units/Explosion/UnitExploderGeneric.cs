using System;
using UnityEngine;

public class UnitExploderGeneric : UnitExploder
{
    [SerializeField] private Sprite _prefabSprite;

    protected override void SetUpPart(Vector3 eulerAngles, Action<UnitExplosionPart> onDistanceReached = null)
    {
        var obj = UnitExplosionPart.Instantiate(_partPrefab, this.transform.position, Quaternion.Euler(eulerAngles), this.transform, _prefabSprite);
        if (onDistanceReached != null)
            obj.OnDistanceReached += onDistanceReached;
    }
}