using UnityEngine;

public class RandomUnit : MonoBehaviour
{
    [SerializeField] private Unit[] _units;
    [SerializeField] private bool _isRandomOnRespawn;

    private bool _isCopied;

    private void Awake()
    {
        if (!_isCopied)
        {
            Spawn();
        }
    }

    public void Spawn(Vector3? position = null)
    {
        position ??= this.transform.position;

        var randomIndex = Random.Range(0, _units.Length);
        var unit = _units[randomIndex];
        unit.gameObject.SetActive(false);
        var instance = GameObject.Instantiate(unit, position.Value, this.transform.rotation, this.transform.parent);
        if (_isRandomOnRespawn)
        {
            CopyComponentTo(instance);
        }
        instance.gameObject.SetActive(true);
        GameObject.Destroy(this.gameObject);
    }

    private void CopyComponentTo(Unit unit)
    {
        // TODO: Respawn after seconds causing incorrect behaviour
        var instanceRandomUnit = unit.gameObject.AddComponent<RandomUnit>();
        instanceRandomUnit._units = _units;
        instanceRandomUnit._isRandomOnRespawn = _isRandomOnRespawn;
        instanceRandomUnit._isCopied = true;
    }
}