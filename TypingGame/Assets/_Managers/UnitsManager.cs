using System.Collections;
using UnityEngine;

public class UnitsManager : Singleton<UnitsManager>
{
    [SerializeField] private GameObject _unitsHolder;

    private void Start()
    {
        GameManager.Instance.OnStateChanging += UpdateUnitsForState;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanging -= UpdateUnitsForState;
    }

    private void UpdateUnitsForState(GameState state)
    {
        if (state.IsEndOfLevel())
        {
            _unitsHolder.SetActive(false);
        }
    }
}