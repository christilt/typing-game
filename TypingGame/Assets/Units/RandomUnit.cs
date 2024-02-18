using System;
using UnityEngine;

public class RandomUnit : MonoBehaviour
{
    [SerializeField] private Unit[] _units;
    [SerializeField] private bool _isRandomOnRespawn;

    private int _replacementCount;

    private void Awake()
    {
        if (_replacementCount == 0)
        {
            ReplaceGameObject();
        }
    }

    public int UnitReplacementCount { get; private set; }

    public bool HasReplacedAUnit => UnitReplacementCount > 0;

    public void ReplaceGameObject(Vector3? position = null)
    {
        position ??= this.transform.position;

        var randomIndex = UnityEngine.Random.Range(0, _units.Length);
        var unit = _units[randomIndex];
        unit.gameObject.SetActive(false);
        var instance = GameObject.Instantiate(unit, position.Value, this.transform.rotation, this.transform.parent);
        _replacementCount++;
        if (TryGetComponent<Unit>(out var _))
        {
            UnitReplacementCount++;
        }
        if (_isRandomOnRespawn)
        {
            CopyComponentTo(instance);
        }
        instance.gameObject.SetActive(true);
        GameObject.Destroy(this.gameObject);
    }

    private void CopyComponentTo(Unit unit)
    {
        var instanceRandomUnit = unit.gameObject.AddComponent<RandomUnit>();
        instanceRandomUnit._units = _units;
        instanceRandomUnit._isRandomOnRespawn = _isRandomOnRespawn;
        instanceRandomUnit._replacementCount = _replacementCount;
        instanceRandomUnit.UnitReplacementCount = UnitReplacementCount;
    }
}