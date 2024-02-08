using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class UnitRespawner : MonoBehaviour
{
    [SerializeField] private float _respawnSeconds;
    [SerializeField] private float _respawnAdditionalRandomSeconds;
    [SerializeField] private Unit _unit;
    [SerializeField] private RespawnMode _mode;

    private bool _isRespawning;
    private float _nextRespawnInSeconds;

    private RandomUnit _optionalRandomUnit;

    private void Start()
    {
        _nextRespawnInSeconds = TotalRespawnSeconds();
        _optionalRandomUnit = GetComponent<RandomUnit>();
    }

    public void RespawnLater()
    {
        //if (_isRespawning)
        //{
        //    Debug.LogWarning($"{nameof(RespawnLater)} prevented because already respawning with {_nextRespawnInSeconds}s remaining");
        //    return;
        //}

        _isRespawning = true;
        _nextRespawnInSeconds = TotalRespawnSeconds();
    }

    public void Stop()
    {
        //if (_isRespawning) 
        //    Debug.Log($"{name} respawning stopped");

        _isRespawning = false;
    }

    private void Update()
    {
        if (!_isRespawning)
            return;

        _nextRespawnInSeconds -= Time.deltaTime;

        if (_nextRespawnInSeconds < 0 )
        {
            Respawn();
            Stop();
        }
    }

    private void Respawn()
    {
        var position = UnitManager.Instance.GetRespawnPosition(_unit, _mode);

        if (_optionalRandomUnit != null)
        {
            _optionalRandomUnit.Spawn(position);
        }
        else
        {
            transform.position = position;
            _unit.BeSpawned();
        }
    }

    private float TotalRespawnSeconds() => _respawnSeconds + Random.Range(0, _respawnAdditionalRandomSeconds);
}
