using System;
using UnityEngine;

public class UnitExploder : MonoBehaviour
{
    // TODO: object pool these
    [SerializeField] protected UnitExplosionPart _partPrefab;

    [SerializeField] private Sprite _partPrefabSprite;
    [SerializeField] protected GameObject _target;

    public void Explode()
    {
        if (_target == null)
            throw new InvalidOperationException("Target already destroyed");

        DestroyTarget();
        SetUpPart(new Vector3(0, 0, -45), DestroyExploder);
        SetUpPart(new Vector3(0, 0, 45));
        SetUpPart(new Vector3(0, 0, -135));
        SetUpPart(new Vector3(0, 0, -225));
    }

    protected virtual void DestroyTarget()
    {
        // TODO make manager parent
        transform.parent = null; // Ensure exploder not destroyed in explosion
        Destroy(_target);
        _target = null;
    }

    protected virtual void SetUpPart(Vector3 eulerAngles, Action<UnitExplosionPart> onDistanceReached = null)
    {
        var obj = UnitExplosionPart.Instantiate(_partPrefab, this.transform.position, Quaternion.Euler(eulerAngles), this.transform, _partPrefabSprite);
        if (onDistanceReached != null)
            obj.OnDistanceReached += onDistanceReached;
    }

    protected virtual void DestroyExploder(UnitExplosionPart part)
    {
        part.OnDistanceReached -= DestroyExploder;
        Destroy(gameObject);
    }
}