using System;
using UnityEngine;

public class UnitExploder : MonoBehaviour
{
    // TODO: object pool these
    [SerializeField] protected UnitExplosionPart _partPrefab;

    [SerializeField] private Sprite _partPrefabSprite;

    public void Explode()
    {
        SetUpPart(new Vector3(0, 0, -45), DestroyPart);
        SetUpPart(new Vector3(0, 0, 45), DestroyPart);
        SetUpPart(new Vector3(0, 0, -135), DestroyPart);
        SetUpPart(new Vector3(0, 0, -225), DestroyPart);
    }

    protected virtual void SetUpPart(Vector3 eulerAngles, Action<UnitExplosionPart> onDistanceReached = null)
    {
        var obj = UnitExplosionPart.Instantiate(_partPrefab, this.transform.position, Quaternion.Euler(eulerAngles), this.transform, _partPrefabSprite);
        if (onDistanceReached != null)
            obj.OnDistanceReached += onDistanceReached;
    }

    protected virtual void DestroyPart(UnitExplosionPart part)
    {
        part.OnDistanceReached -= DestroyPart;
        Destroy(part.gameObject);
    }
}