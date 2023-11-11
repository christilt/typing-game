using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] private GameObject _unitsHolder;

    private readonly HashSet<Collectable> _collectables = new();
    private readonly HashSet<Enemy> _enemies = new();

    private void Start()
    {
        GameManager.Instance.OnStateChanging += UpdateUnitsForState;
    }

    public bool TryRegister(Collectable collectable)
    {
        if (_collectables.Contains(collectable))
            return false;

        _collectables.Add(collectable);
        return true;
    }

    public bool TryRegister(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
            return false;

        _enemies.Add(enemy);
        return true;
    }

    public void KillAllEnemies()
    {
        foreach(var enemy in _enemies)
            enemy.BeDestroyed();
    }

    public void ChangeUnitSpeed(float multiplier, float durationSeconds)
    {
        foreach (var collectable in _collectables)
            collectable.ChangeSpeed(multiplier, durationSeconds);

        foreach (var enemy in _enemies)
            enemy.ChangeSpeed(multiplier, durationSeconds);
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