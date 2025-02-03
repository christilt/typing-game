using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnPositioner : MonoBehaviour, ILoadsSlowly
{
    [SerializeField] private LayerMask _positionObstaclesMask;
    [SerializeField] private float _positionObstaclesCheckRadius;
    [SerializeField] private int _additionalRandomPositionsCount;

    private object _respawnPositionsTakenLock = new();

    private readonly HashSet<AllowedPosition> _allowedPositions = new();
    private readonly HashSet<Vector3> _allowedPositionsTaken = new();
    private readonly Dictionary<int, Vector3> _reservedPositions = new();

    private void Start()
    {
        KeyTiles.Instance.OnInitialised += AddRandomTilePositions;
    }

    private void OnDestroy()
    {
        if (KeyTiles.Instance != null)
            KeyTiles.Instance.OnInitialised -= AddRandomTilePositions;
    }

    private List<AllowedPosition> AllowedPositionsAvailable => _allowedPositions
        .Where(l => _allowedPositionsTaken.All(p => p != l.PositionWithCentre.Position))
        .ToList();

    public bool IsLoaded { get; private set; }

    public void RegisterStartPosition(Unit unit)
    {
        if (unit.HasBoundary)
        {
            _reservedPositions.Add(unit.transform.GetInstanceID(), unit.transform.position);
            return;
        }

        _allowedPositions.Add(new AllowedPosition(unit.PositionWithCentre, isRandom: false));
    }

    public void UnregisterStartPosition(Unit unit)
    {
        if (unit.HasBoundary)
        {
            _reservedPositions.Remove(unit.transform.GetInstanceID());
            return;
        }
    }

    public Vector3 GetRespawnPosition(Unit unit, RespawnMode mode)
    {
        if (unit.HasBoundary && _reservedPositions.ContainsKey(unit.transform.GetInstanceID()))
        {
            return _reservedPositions[unit.transform.GetInstanceID()];
        }

        lock (_respawnPositionsTakenLock) // Ensure respawn positions taken synchronously within the same frame
        {
            var positionInfo = GetRespawnPositionWithoutLock(mode);
            _allowedPositionsTaken.Add(positionInfo.Position);

            StartCoroutine(RemovePositionTakenCoroutine(positionInfo.Position));

            return positionInfo.Position;
        }

        IEnumerator RemovePositionTakenCoroutine(Vector3 position)
        {
            yield return new WaitForEndOfFrame();

            _allowedPositionsTaken.Remove(position);
        }
    }

    private RespawnPositionInfo GetRespawnPositionWithoutLock(RespawnMode mode)
    {
        var respawnPositionInfos = GetRespawnPositionsWithoutLock()
            .OrderBy(respawnPositionInfo => respawnPositionInfo.ObstaclesNearbyCount)
            .ThenBy(respawnPositionInfo => respawnPositionInfo.IsRandom);

        if (mode == RespawnMode.ClosestToPlayer)
        {
            respawnPositionInfos = respawnPositionInfos.ThenBy(respawnPositionInfo => respawnPositionInfo.DistanceFromPlayer);
        }
        else if (mode == RespawnMode.FurthestFromPlayer)
        {
            respawnPositionInfos = respawnPositionInfos.ThenByDescending(respawnPositionInfo => respawnPositionInfo.DistanceFromPlayer);
        }

        //Debug.Log($"Respawn positions for {name}: {string.Join(Environment.NewLine, respawnPositionInfos.Select(rpi => rpi.ToString()))}");

        return respawnPositionInfos.First();
    }

    private List<RespawnPositionInfo> GetRespawnPositionsWithoutLock()
    {
        var list = new List<RespawnPositionInfo>();
        foreach (var allowed in AllowedPositionsAvailable)
        {
            var colliders = Physics2D.OverlapCircleAll(allowed.PositionWithCentre.Centre, _positionObstaclesCheckRadius, _positionObstaclesMask);
            var distanceFromPlayer = Vector2.Distance(allowed.PositionWithCentre.Centre, Player.Instance.Centre);
            if (distanceFromPlayer < _positionObstaclesCheckRadius)
                continue; // Exclude positions right next to player e.g. when all positions have obstacles

            list.Add(new RespawnPositionInfo(allowed.PositionWithCentre.Position, allowed.IsRandom, colliders.Length, distanceFromPlayer));
        }
        return list;
    }

    // Reduce the chance of collisions when choosing respawn positions by adding random tile positions
    private void AddRandomTilePositions()
    {
        var randomPositionsAllowed = KeyTiles.Instance.GetRandomTiles(_additionalRandomPositionsCount)
            .Select(t => new PositionWithCentre(t.Position, t.Centre));

        foreach (var position in randomPositionsAllowed)
        {
            _allowedPositions.Add(new AllowedPosition(position, isRandom: true));
        }

        IsLoaded = true;
    }

    private struct AllowedPosition
    {
        public AllowedPosition(PositionWithCentre positionWithCentre, bool isRandom)
        {
            PositionWithCentre = positionWithCentre;
            IsRandom = isRandom;
        }

        public PositionWithCentre PositionWithCentre { get; }
        public bool IsRandom { get; }
    }
}

public struct RespawnPositionInfo
{
    public RespawnPositionInfo(Vector3 position, bool isRandom, int unitsNearbyCount, float distanceFromPlayer)
    {
        Position = position;
        IsRandom = isRandom;
        ObstaclesNearbyCount = unitsNearbyCount;
        DistanceFromPlayer = distanceFromPlayer;
    }

    public Vector3 Position { get; }
    public bool IsRandom { get; }
    public int ObstaclesNearbyCount { get; }
    public float DistanceFromPlayer { get; }
    public override string ToString()
    {
        return $"{Position}; IsRandom:{IsRandom}; {ObstaclesNearbyCount} obstacles; {DistanceFromPlayer} from player";
    }
}

public enum RespawnMode
{
    Random,
    ClosestToPlayer,
    FurthestFromPlayer
}