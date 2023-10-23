//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

// TODO - Naive - use AStar instead
//public class PathFinder
//{
//    private readonly HashSet<Vector2> _positions;
//    private readonly Tilemap _tilemap;
//    private readonly LayerMask _layerMask;

//    public PathFinder(HashSet<Vector2> positions, Tilemap tilemap)
//    {
//        _positions = positions;
//        _tilemap = tilemap;
//    }

//    public bool TryGetDirection(Vector2 from, Vector2 to, IEnumerable<Vector2> directionOptions, int attempts, out Vector2 direction)
//    {
//        if (attempts < 1)
//            throw new ArgumentOutOfRangeException(nameof(attempts), attempts, "");

//        foreach(var directionOption in directionOptions)
//        {

//        }

//        // TODO
//    }

//    private bool IsTowardsTarget(Vector2 from, Vector2 to, Vector2 direction)
//    {
//        if (!FindTarget(from, to, direction, out var positionsTowards))
//            return false;

//        return true; // TODO
//    }

//    private bool FindTarget(Vector2 from, Vector2 to, Vector2 direction, out List<Vector2> positionsTowards)
//    {
//        positionsTowards = null;

//        var fromCellPosition = (Vector2Int)_tilemap.WorldToCell(from);
//        if (!_positions.Contains(fromCellPosition))
//        {
//            Debug.LogError($"{nameof(PathFinder)} positions does not contain source position of {fromCellPosition}");
//            return false;
//        }

//        var toCellPosition = (Vector2Int)_tilemap.WorldToCell(to);
//        if (!_positions.Contains(toCellPosition))
//        {
//            Debug.LogError($"{nameof(PathFinder)} positions does not contain destination position of {toCellPosition}");
//            return false;
//        }

//        positionsTowards = new List<Vector2>();

//        Vector2 currentPosition = fromCellPosition;
//        currentPosition += direction;
//        while (
//            _positions.Contains(currentPosition)
//            && currentPosition != toCellPosition)
//        {
//            positionsTowards.Add(currentPosition);
//            currentPosition += direction;
//        }

//        return true;
//    }

//    private class FinderResult
//    {
//        public FinderResult(Vector2Int cellPosition, bool isTarget)
//        {
//            CellPosition = cellPosition;
//            IsTarget = isTarget;
//        }

//        public Vector2Int CellPosition { get; }
//        public bool IsTarget { get; }
//}