using System;
using UnityEngine;

public class UnitExploder : MonoBehaviour
{
    // TODO: object pool these
    [SerializeField] private UnitExplosionPart _partPrefab;

    [SerializeField] private GameObject _target;

    public void Explode()
    {
        if (_target == null)
            throw new InvalidOperationException("Target already destroyed");

        DestroyTarget();
        SetUpPart(_partPrefab, new Vector3(0, 0, -45), DestroyExploder);
        SetUpPart(_partPrefab, new Vector3(0, 0, 45));
        SetUpPart(_partPrefab, new Vector3(0, 0, -135));
        SetUpPart(_partPrefab, new Vector3(0, 0, -225));
    }

    private void DestroyTarget()
    {
        // TODO make manager parent
        transform.parent = null; // Ensure exploder not destroyed in explosion
        this.DestroyWithChildren(_target);
        _target = null;
    }

    private void SetUpPart(UnitExplosionPart explosionPartPrefab, Vector3 eulerAngles, Action<UnitExplosionPart> onDistanceReached = null)
    {
        var obj = Instantiate(explosionPartPrefab, this.transform.position, Quaternion.Euler(eulerAngles), this.transform);
        if (onDistanceReached != null)
            obj.OnDistanceReached += onDistanceReached;
    }

    private void DestroyExploder(UnitExplosionPart part)
    {
        part.OnDistanceReached -= DestroyExploder;
        this.DestroyWithChildren(gameObject);
    }
}