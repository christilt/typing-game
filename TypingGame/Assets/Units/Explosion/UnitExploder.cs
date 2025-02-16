using System;
using UnityEngine;

public class UnitExploder : MonoBehaviour, ILoadsSlowly
{
    [SerializeField] private SpriteRenderer _partPrefabSpriteRenderer;
    private int _explosionPartPoolsKey;

    private Sprite _partPrefabSprite;

    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        _partPrefabSprite = _partPrefabSpriteRenderer.sprite;
    }

    private void Start()
    {
        UnitManager.Instance.TryRegisterExplosionParts(_partPrefabSpriteRenderer, out _explosionPartPoolsKey, () =>
        {
            IsLoaded = true;
        });
    }

    public void Explode()
    {
        GetAndSetUpPart(new Vector3(0, 0, -45), ReleasePart);
        GetAndSetUpPart(new Vector3(0, 0, 45), ReleasePart);
        GetAndSetUpPart(new Vector3(0, 0, -135), ReleasePart);
        GetAndSetUpPart(new Vector3(0, 0, -225), ReleasePart);
    }

    protected virtual void GetAndSetUpPart(Vector3 eulerAngles, Action<UnitExplosionPart> onDistanceReached = null)
    {
        if (!UnitManager.Instance.TryGetExplosionPart(_explosionPartPoolsKey, out var part))
        {
            Debug.LogError($"{nameof(UnitManager.TryGetExplosionPart)} failed");
        }
        part.transform.parent = this.transform;
        part.transform.position = this.transform.position;
        part.transform.rotation = Quaternion.Euler(eulerAngles);
        part.gameObject.SetActive(true);
        if (onDistanceReached != null)
            part.OnDistanceReached += onDistanceReached;
    }

    protected virtual void ReleasePart(UnitExplosionPart part)
    {
        part.OnDistanceReached -= ReleasePart;
        part.transform.parent = null;
        part.transform.position = default;
        part.transform.rotation = default;
        part.gameObject.SetActive(false);
        if (!UnitManager.Instance.TryReleaseExplosionPart(_explosionPartPoolsKey, part))
        {
            Debug.LogError($"{nameof(UnitManager.TryReleaseExplosionPart)} failed");
        }
    }
}