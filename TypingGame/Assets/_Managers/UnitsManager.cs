using System.Collections;
using UnityEngine;

public class UnitsManager : Singleton<UnitsManager>
{
    [SerializeField] private GameObject _unitsHolder;

    public void SpawnUnits()
    {
        Debug.Log("Starting: Spawning Units");
        StartCoroutine(SpawnUnitsCoroutine());
    }

    private IEnumerator SpawnUnitsCoroutine()
    {
        // TODO remove
        yield return new WaitForSeconds(2);

        _unitsHolder.SetActive(true);
    }
}