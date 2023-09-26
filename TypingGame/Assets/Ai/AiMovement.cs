using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Make this clever
// TODO: Bug: sometimes cutting corners (use waypoints?)
// TODO: Bug: sometimes changing direction when colliding in same direction
// TODO: Bug: occasionally overlapping when trapped
public class AiMovement : MonoBehaviour
{
    [SerializeField] private float _movesPerSecond; // TODO: Have this in WPM
    [SerializeField] private Tilemap _allowedTiles;
    [SerializeField] private LayerMask _colliderLayers;
    [SerializeField] private Transform _centre;

    private HashSet<Vector2Int> _allowedPositions;

    private Vector2Int[] _previousDirectionOptions;
    private Vector2Int _previousDirection = Vector2Int.zero;
    private Vector2Int _direction = Vector2Int.zero;
    private HashSet<Vector2Int> _collisionDirections = new();

    private Movement? _centreMovement;

    private Vector2Int CellPosition => GetCell(_centre.position);
    private Vector2Int PreviousDirectionOpposite => _previousDirection * -1;

    void Awake()
    {
        _allowedPositions = new HashSet<Vector2Int>(_allowedTiles.GetPositions());
    }

    private void Update()
    {
        if (_centreMovement is null || _centreMovement.Value.IsExceededBy(_centre.position) || _collisionDirections.Count > 0)
        {
            UpdateDirections();
            if (!TryGetNeighbourCell(CellPosition, _direction, out var neighbourCell))
            {
                Debug.LogError($"Could not get neighbour of cell {CellPosition} in direction {_direction}!");
                return;
            }
            _centreMovement = new Movement(_centre.position, GetCellCentre(neighbourCell));
        }
        transform.position += (_centreMovement.Value.Direction * Time.deltaTime * _movesPerSecond);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherLayer = collision.gameObject.layer;
        if (!_colliderLayers.HasLayer(otherLayer))
            return;

        _collisionDirections.Add(_direction);
        var colliderDirection = GetDirectionToPosition(collision.gameObject.transform.position);
        _collisionDirections.Add(colliderDirection);
    }

    private void UpdateDirections()
    {
        Vector2Int[] directionOptions = GetDirectionOptions();
        var shouldChooseDirection = !IsPreviousDirectionAvailable(directionOptions) || IsNewDirectionAvailable(directionOptions);
        if (shouldChooseDirection)
        {
            _previousDirectionOptions = directionOptions;
            _direction = ChooseDirection(directionOptions);
            _previousDirection = _direction;
        }
        else
        {
            _direction = _previousDirection;
        }
        _collisionDirections.Clear();
    }

    private Vector2Int[] GetDirectionOptions()
    {
        return GetNeighbourCells(_centre.position)
            .Select(GetDirectionToCell)
            .Where(d => d != PreviousDirectionOpposite)
            .Except(_collisionDirections)
            .ToArray();
    }

    private Vector2Int ChooseDirection(Vector2Int[] directionOptions)
    {
        if (!directionOptions.Any())
            return PreviousDirectionOpposite;

        return directionOptions[UnityEngine.Random.Range(0, directionOptions.Length)];
    }


    private bool IsPreviousDirectionAvailable(Vector2Int[] directionOptions) => directionOptions.Contains(_previousDirection);
    private bool IsNewDirectionAvailable(Vector2Int[] directionOptions) => _previousDirectionOptions is null || directionOptions.Except(_previousDirectionOptions).Any();
    private Vector2Int GetDirectionToPosition(Vector3 position) => GetDirectionToCell(GetCell(position));
    private Vector2Int GetDirectionToCell(Vector2Int cell) => cell - CellPosition;
    private IEnumerable<Vector2Int> GetNeighbourCells(Vector3 position) => GetNeighbourCells(GetCell(position));
    private IEnumerable<Vector2Int> GetNeighbourCells(Vector2Int cell)
    {
        if (_allowedPositions.TryGetValue(cell + Vector2Int.up, out Vector2Int upPosition))
            yield return upPosition;
        if (_allowedPositions.TryGetValue(cell + Vector2Int.down, out Vector2Int downPosition))
            yield return downPosition;
        if (_allowedPositions.TryGetValue(cell + Vector2Int.left, out Vector2Int leftPosition))
            yield return leftPosition;
        if (_allowedPositions.TryGetValue(cell + Vector2Int.right, out Vector2Int rightPosition))
            yield return rightPosition;
    }
    private bool TryGetNeighbourCell(Vector2Int cell, Vector2Int direction, out Vector2Int neighbourCell)
    {
        return _allowedPositions.TryGetValue(cell + direction, out neighbourCell);
    }

    // TODO
    // WorldToCell appears to not give the right answer here
    //private Vector2Int GetCell(Vector3 position) => new Vector2Int((int)Mathf.Round(position.x), (int)Mathf.Round(position.y));
    private Vector2Int GetCell(Vector3 position) => (Vector2Int)_allowedTiles.WorldToCell(position);
    private Vector3 GetCellCentre(Vector2Int cell) => new Vector3(cell.x, cell.y, 0) + _centre.localPosition;


    [Serializable]
    private struct Movement
    {
        public Movement(Vector3 from, Vector3 to)
        {
            From = from;
            To = to;
        }

        public Vector3 From { get; }
        public Vector3 To { get; }
        public Vector3 Direction => (To - From).normalized;
        public bool IsExceededBy(Vector3 position) => (position - From).magnitude > (To - From).magnitude;
    }
}
