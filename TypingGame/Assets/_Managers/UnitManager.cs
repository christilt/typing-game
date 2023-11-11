using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] private GameObject _unitsHolder;

    private readonly HashSet<Enemy> _enemies = new();

    private void Start()
    {
        GameManager.Instance.OnStateChanging += UpdateUnitsForState;
    }

    public bool TryRegister(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
            return false;

        _enemies.Add(enemy);
        return true;
    }

    public bool KillAllEnemies()
    {
        if (_enemies.Count == 0) 
            return false;

        foreach(var enemy in _enemies) 
        {
            enemy.BeDestroyed();
        }

        return true;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
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