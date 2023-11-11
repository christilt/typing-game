using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnPositioner : MonoBehaviour
{
    [SerializeField] private LayerMask _positionObstaclesMask;
    [SerializeField] private float _positionObstaclesCheckRadius;

    private object _respawnPositionsTakenLock = new();

    private readonly HashSet<PositionWithCentre> _startLocations = new();
    private readonly HashSet<Vector3> _startPositionsTaken = new();

    private List<PositionWithCentre> StartLocationsAvailable => _startLocations
        .Where(l => _startPositionsTaken.All(p => p != l.Position))
        .ToList();

    public void RegisterStartPosition(Unit unit)
    {
        _startLocations.Add(unit.PositionWithCentre);
    }

    public Vector3 GetRespawnPosition(RespawnMode mode)
    {
        lock (_respawnPositionsTakenLock) // Ensure a respawn positions taken synchronously within the same frame
        {
            var positionInfo = GetRespawnPositionWithoutLock(mode);
            _startPositionsTaken.Add(positionInfo.Position);

            StartCoroutine(RemovePositionTakenCoroutine(positionInfo.Position));

            return positionInfo.Position;
        }

        IEnumerator RemovePositionTakenCoroutine(Vector3 position)
        {
            yield return new WaitForEndOfFrame();

            _startPositionsTaken.Remove(position);
        }
    }

    private RespawnPositionInfo GetRespawnPositionWithoutLock(RespawnMode mode)
    {
        var respawnPositionInfos = GetRespawnPositionsWithoutLock()
            .OrderBy(respawnPositionInfo => respawnPositionInfo.ObstaclesNearbyCount);

        if (mode == RespawnMode.ClosestToPlayer)
        {
            respawnPositionInfos = respawnPositionInfos.ThenBy(respawnPositionInfo => respawnPositionInfo.DistanceFromPlayer);
        }
        else if (mode == RespawnMode.FurthestFromPlayer)
        {
            respawnPositionInfos = respawnPositionInfos.ThenByDescending(respawnPositionInfo => respawnPositionInfo.DistanceFromPlayer);
        }

        //Debug.Log($"Respawn positions for {name}: {string.Join(", ", respawnPositionInfos.Select(rpi => rpi.ToString()))}");

        return respawnPositionInfos.First();
    }

    private List<RespawnPositionInfo> GetRespawnPositionsWithoutLock()
    {
        var list = new List<RespawnPositionInfo>();
        foreach (var location in StartLocationsAvailable)
        {
            var colliders = Physics2D.OverlapCircleAll(location.Centre, _positionObstaclesCheckRadius, _positionObstaclesMask);
            var distance = Vector2.Distance(location.Centre, Player.Instance.Centre);
            list.Add(new RespawnPositionInfo(location.Position, colliders.Length, distance));
        }
        return list;
    }
}

public struct RespawnPositionInfo
{
    public RespawnPositionInfo(Vector3 position, int unitsNearbyCount, float distanceFromPlayer)
    {
        Position = position;
        ObstaclesNearbyCount = unitsNearbyCount;
        DistanceFromPlayer = distanceFromPlayer;
    }

    public Vector3 Position { get; }
    public int ObstaclesNearbyCount { get; }
    public float DistanceFromPlayer { get; }
    public override string ToString()
    {
        return $"{Position}; {ObstaclesNearbyCount} obstacles; {DistanceFromPlayer} from player";
    }
}

public enum RespawnMode
{
    Random,
    ClosestToPlayer,
    FurthestFromPlayer
}