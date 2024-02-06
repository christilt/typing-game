using UnityEngine;
using UnityEngine.UIElements;

public class UnitRespawner : MonoBehaviour
{
    [SerializeField] private float _respawnSeconds;
    [SerializeField] private float _respawnAdditionalRandomSeconds;
    [SerializeField] private Unit _unit;
    [SerializeField] private RespawnMode _mode;

    private bool _isRespawning;
    private float _nextRespawnInSeconds;

    private void Start()
    {
        _nextRespawnInSeconds = TotalRespawnSeconds();
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
        transform.position = UnitManager.Instance.GetRespawnPosition(_unit, _mode);
        _unit.BeSpawned();
    }

    private float TotalRespawnSeconds() => _respawnSeconds + Random.Range(0, _respawnAdditionalRandomSeconds);
}
