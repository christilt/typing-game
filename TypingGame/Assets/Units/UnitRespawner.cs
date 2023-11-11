using UnityEngine;

public class UnitRespawner : MonoBehaviour
{
    [SerializeField] private float _respawnSeconds;
    [SerializeField] private Unit _unit;

    private bool _isRespawning;
    private float _nextRespawnInSeconds;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
        _nextRespawnInSeconds = _respawnSeconds;
    }

    public void RespawnLater()
    {
        if (_isRespawning)
        {
            Debug.LogWarning($"{nameof(RespawnLater)} prevented because already respawning with {_nextRespawnInSeconds}s remaining");
            return;
        }

        _isRespawning = true;
        _nextRespawnInSeconds = _respawnSeconds;
    }

    public void Stop()
    {
        if (_isRespawning) 
            Debug.Log($"{name} respawning stopped");

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
        // TODO: Get other respawn positions from UnitManager
        transform.position = _startPosition;
        _unit.BeSpawned();
    }

}