using UnityEngine;

public class RandomUnit : MonoBehaviour
{
    [SerializeField] private Unit[] _units;

    private void Awake()
    {
        var randomIndex = Random.Range(0, _units.Length);
        var unit = _units[randomIndex];
        GameObject.Instantiate(unit, this.transform.position, this.transform.rotation, this.transform.parent);
        GameObject.Destroy(this.gameObject);
    }
}