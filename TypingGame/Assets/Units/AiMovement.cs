using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Make this clever
// TODO: Bug: occasionally overlapping when trapped
[RequireComponent(typeof(Rigidbody2D))]
public class AiMovement : MonoBehaviour
{
    private const float CornerCuttingDistanceTolerated = 0.25f;

    [SerializeField] private float _movesPerSecond; // TODO: Have this in WPM
    [SerializeField] private Tilemap _allowedTiles;
    [SerializeField] private LayerMask _deflectFromCollisionsInLayers;
    [SerializeField] private Transform _centre;
    [SerializeField] private Mode _mode; // TODO

    private Rigidbody2D _rigidbody;

    private HashSet<Vector2Int> _allowedPositions;

    private Vector2Int[] _previousDirectionOptions;
    private Vector2Int _previousDirection = Vector2Int.zero;
    private HashSet<Vector2Int> _deflectedFromDirections = new();

    private Movement? _centreMovement;

    public Vector2Int Direction { get; private set; } = Vector2Int.zero;
    private Vector2Int CellPosition => GetCell(_centre.position);
    private Vector2Int PreviousDirectionOpposite => _previousDirection * -1;

    private void Awake()
    {
        _allowedPositions = new HashSet<Vector2Int>(_allowedTiles.GetPositions());
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_centreMovement is null || _centreMovement.Value.IsExceededBy(_centre.position) || _deflectedFromDirections.Count > 0)
        {
            UpdateDirections();
            if (!TryGetNeighbourCell(CellPosition, Direction, out var neighbourCell))
            {
                // TODO: Understand why this happens
                Debug.LogError($"Could not get neighbour of cell {CellPosition} in direction {Direction}!");
                return;
            }
            _centreMovement = new Movement(_centre.position, GetCellCentre(neighbourCell));
        }

        if (_centreMovement is null)
            return;

        var newPosition = transform.position + (_centreMovement.Value.Direction * Time.deltaTime * _movesPerSecond);
        _rigidbody.MovePosition(newPosition);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherLayer = collision.gameObject.layer;
        if (!_deflectFromCollisionsInLayers.HasLayer(otherLayer))
            return;

        var otherDirection = (collision.gameObject.transform.position - transform.position).normalized;
        var otherGridDirection = Round(otherDirection);
        var otherStrictGridDirections = GetStrictGridDirectionsTo(otherGridDirection);
        foreach (var direction in otherStrictGridDirections)
            _deflectedFromDirections.Add(direction);

        // TODO remove
        //Debug.Log($"[{name}]:otherDirection: {otherDirection}; relative velocity magnitude: {collision.relativeVelocity.magnitude}; _collisionPushDirections: {string.Join(", ", _collisionPushDirections)}");
    }
    private static Vector2Int Round(Vector2 velocity) => new Vector2Int((int)Math.Round(velocity.x), (int)Math.Round(velocity.y));
    private static IEnumerable<Vector2Int> GetStrictGridDirectionsTo(Vector2Int direction)
    {
        if (HasAnyMatchingComponent(direction, Vector2Int.up))
            yield return Vector2Int.up;
        if (HasAnyMatchingComponent(direction, Vector2Int.down))
            yield return Vector2Int.down;
        if (HasAnyMatchingComponent(direction, Vector2Int.left))
            yield return Vector2Int.left;
        if (HasAnyMatchingComponent(direction, Vector2Int.right))
            yield return Vector2Int.right;
    }
    private static bool HasAnyMatchingComponent(Vector2Int a, Vector2Int b) => (a.x != 0 && a.x == b.x) ||( a.y != 0 && a.y == b.y);

    private void UpdateDirections()
    {
        Vector2Int[] directionOptions = GetDirectionOptions();
        var shouldChooseDirection = !IsPreviousDirectionAvailable(directionOptions) || IsNewDirectionAvailable(directionOptions);
        if (shouldChooseDirection)
        {
            _previousDirectionOptions = directionOptions;
            Direction = ChooseDirection(directionOptions);
            _previousDirection = Direction;
        }
        else
        {
            Direction = _previousDirection;
        }
        _deflectedFromDirections.Clear();
    }

    private Vector2Int[] GetDirectionOptions()
    {
        if (_deflectedFromDirections.Count > 0 && !_centreMovement.Value.IsAlmostExceededBy(_centre.position, CornerCuttingDistanceTolerated))
        {
            var deflectedToDirections = _deflectedFromDirections.Select(d => d * -1);

            return GetNeighbourCells(_centre.position)
                .Select(GetDirectionToCell)
                .Intersect(deflectedToDirections)
                .ToArray();
        }

        return GetNeighbourCells(_centre.position)
            .Select(GetDirectionToCell)
            .Where(d => d != PreviousDirectionOpposite)
            .Except(_deflectedFromDirections)
            .ToArray();
    }

    private Vector2Int ChooseDirection(Vector2Int[] directionOptions)
    {
        if (!directionOptions.Any())
            return PreviousDirectionOpposite;

        // TODO make this clever

        return directionOptions[UnityEngine.Random.Range(0, directionOptions.Length)];
    }


    private bool IsPreviousDirectionAvailable(Vector2Int[] directionOptions) => directionOptions.Contains(_previousDirection);
    private bool IsNewDirectionAvailable(Vector2Int[] directionOptions) => _previousDirectionOptions is null || directionOptions.Except(_previousDirectionOptions).Any();
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

    private Vector2Int GetCell(Vector3 position) => (Vector2Int)_allowedTiles.WorldToCell(position);
    private Vector3 GetCentre(Vector3 position) => position + _centre.localPosition;
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
        public bool IsAlmostExceededBy(Vector3 position, float distanceTolerated) => ((position - From).magnitude + distanceTolerated) > (To - From).magnitude;
        public bool IsExceededBy(Vector3 position) => (position - From).magnitude > (To - From).magnitude;
    }

    private enum Mode
    {
        Random,
        ChasePlayer_1Line,
        EvadePlayer_1Line
    }
}
