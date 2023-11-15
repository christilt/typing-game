using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RespawnPositioner))]
public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] private GameObject _unitsHolder;
    
    private RespawnPositioner _respawnPositioner;

    private readonly HashSet<Collectable> _collectables = new();
    private readonly HashSet<Enemy> _enemies = new();

    protected override void Awake()
    {
        base.Awake();
        _respawnPositioner = GetComponent<RespawnPositioner>();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanging += UpdateUnitsForState;
    }

    public bool TryRegister(Collectable collectable)
    {
        if (_collectables.Contains(collectable))
            return false;

        _collectables.Add(collectable);
        _respawnPositioner.RegisterStartPosition(collectable);
        return true;
    }

    public bool TryRegister(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
            return false;

        _enemies.Add(enemy);
        _respawnPositioner.RegisterStartPosition(enemy);
        return true;
    }

    public void KillEnemies()
    {
        foreach(var enemy in _enemies)
            enemy.BeDestroyed();
    }

    public void FrightenEnemies()
    {
        foreach (var enemy in _enemies)
            enemy.FearPlayer();
    }

    public void EmboldenEnemies()
    {
        foreach (var enemy in _enemies)
            enemy.DoNotFearPlayer();
    }

    public void ChangeUnitSpeed(float multiplier)
    {
        foreach (var collectable in _collectables)
            collectable.ChangeSpeed(multiplier);

        foreach (var enemy in _enemies)
            enemy.ChangeSpeed(multiplier);
    }

    public void ResetUnitSpeed()
    {
        foreach (var collectable in _collectables)
            collectable.ResetSpeed();

        foreach (var enemy in _enemies)
            enemy.ResetSpeed();
    }

    public Vector3 GetRespawnPosition(RespawnMode mode) => _respawnPositioner.GetRespawnPosition(mode);

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