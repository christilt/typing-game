using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Make this clever
public class AiMovement : MonoBehaviour
{
    [SerializeField] private float _movesPerSecond; // TODO: Have this in WPM
    [SerializeField] private Tilemap _allowedTiles;

    private HashSet<Vector2Int> _allowedPositions;

    private Vector2[] _previousDirectionOptions;
    private Vector2 _previousDirection = Vector2.up;
    private Vector2 _direction = Vector2.up;

    private Movement? _worldMovement;

    private Vector2Int CellPosition => ToVector2Int(transform.position);
    private Vector2 PreviousDirectionOpposite => _previousDirection * -1;

    void Awake()
    {
        _allowedPositions = new HashSet<Vector2Int>(_allowedTiles.GetPositions());
    }

    private void Update()
    {
        if (_worldMovement is null || _worldMovement.Value.IsExceededBy(transform.position))
        {
            UpdateDirections();
            var nextMovePosition2 = CellPosition + _direction;
            _worldMovement = new Movement(transform.position, nextMovePosition2);
        }
        transform.position += (_worldMovement.Value.Direction * Time.deltaTime * _movesPerSecond);
    }

    private void UpdateDirections()
    {
        Vector2[] directionOptions = GetDirectionOptions();
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
    }

    private Vector2[] GetDirectionOptions()
    {
        return GetNeighboursOf(ToVector2Int(transform.position))
            .Select(MapPositionToDirection)
            .Where(d => d != PreviousDirectionOpposite)
            .ToArray();
    }

    private Vector2 ChooseDirection(Vector2[] directionOptions)
    {
        if (!directionOptions.Any())
            return PreviousDirectionOpposite;

        return directionOptions[UnityEngine.Random.Range(0, directionOptions.Length)];
    }

    private Vector2 MapPositionToDirection(Vector2 position) => position - CellPosition;

    private bool IsPreviousDirectionAvailable(Vector2[] directionOptions) => directionOptions.Contains(_previousDirection);
    private bool IsNewDirectionAvailable(Vector2[] directionOptions) => _previousDirectionOptions is null || directionOptions.Except(_previousDirectionOptions).Any();
    private IEnumerable<Vector2> GetNeighboursOf(Vector2Int position)
    {
        if (_allowedPositions.TryGetValue(position + Vector2Int.up, out Vector2Int upPosition))
            yield return upPosition;
        if (_allowedPositions.TryGetValue(position + Vector2Int.down, out Vector2Int downPosition))
            yield return downPosition;
        if (_allowedPositions.TryGetValue(position + Vector2Int.left, out Vector2Int leftPosition))
            yield return leftPosition;
        if (_allowedPositions.TryGetValue(position + Vector2Int.right, out Vector2Int rightPosition))
            yield return rightPosition;
    }

    // WorldToCell appears to not give the right answer here
    private Vector2Int ToVector2Int(Vector3 position) => new Vector2Int((int)Mathf.Round(position.x), (int)Mathf.Round(position.y));

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
