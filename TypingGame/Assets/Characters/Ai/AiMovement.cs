using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Make this clever
// TODO: Movement should be smooth
public class AiMovement : MonoBehaviour
{
    [SerializeField] private float _movesPerSecond; // TODO: Have this in WPM
    [SerializeField] private Tilemap _allowedTiles;

    private HashSet<Vector2Int> _allowedPositions;

    private float _secondsUntilNextMove;
    private float _secondsBetweenMoves;

    private Vector2Int[] _lastDirectionOptions;
    private Vector2Int _lastDirection = Vector2Int.up;
    private Vector2Int _direction = Vector2Int.up;

    private Vector2Int _moveFromPosition;
    private Vector2Int _moveToPosition;
    private Vector3 _lerpFromPosition;
    private Vector3 _lerpToPosition;

    private Vector2Int LastDirectionOpposite => _lastDirection * -1;

    void Awake()
    {
        _secondsBetweenMoves = 1f / _movesPerSecond;
        _allowedPositions = new HashSet<Vector2Int>(_allowedTiles.GetPositions());
        var initialMoveFromPosition = (Vector2Int)_allowedTiles.WorldToCell(transform.position);
        _moveFromPosition = initialMoveFromPosition;
        _moveToPosition = initialMoveFromPosition;
    }

    private float SecondsSinceLastMove => _secondsBetweenMoves - _secondsUntilNextMove;

    private void Update()
    {
        // TODO: if (_secondsUntilNextMove <= 0) - assumes will always reach destination at this time - work by distance, not time
        // TODO: _moveFromPosition = _moveToPosition - assumes will always reach destination - what about collisions?
        _secondsUntilNextMove -= Time.deltaTime;
        if (_secondsUntilNextMove <= 0)
        {
            _moveFromPosition = _moveToPosition; 
            _lerpFromPosition = new Vector3(_moveFromPosition.x, _moveFromPosition.y);
            UpdateDirections();
            _moveToPosition = _moveFromPosition + _direction;
            _lerpToPosition = new Vector3(_moveToPosition.x, _moveToPosition.y);
            _secondsUntilNextMove = _secondsBetweenMoves;
        }
        transform.position = Vector2.Lerp(_lerpFromPosition, _lerpToPosition, SecondsSinceLastMove);
    }

    private void UpdateDirections()
    {
        Vector2Int[] directionOptions = GetDirectionOptions();
        var shouldChooseDirection = !IsLastDirectionAvailable(directionOptions) || IsNewDirectionAvailable(directionOptions);
        if (shouldChooseDirection)
        {
            _lastDirectionOptions = directionOptions;
            _direction = ChooseDirection(directionOptions);
            _lastDirection = _direction;
        }
        else
        {
            _direction = _lastDirection;
        }
    }

    private Vector2Int[] GetDirectionOptions()
    {
        return GetNeighboursOf(_moveFromPosition)
            .Select(MapPositionToDirection)
            .Where(d => d != LastDirectionOpposite)
            .ToArray();
    }

    private Vector2Int ChooseDirection(Vector2Int[] directionOptions)
    {
        if (!directionOptions.Any())
            return LastDirectionOpposite;

        return directionOptions[Random.Range(0, directionOptions.Length)];
    }

    private Vector2Int MapPositionToDirection(Vector2Int position) => position - _moveFromPosition;

    private bool IsLastDirectionAvailable(Vector2Int[] directionOptions) => directionOptions.Contains(_lastDirection);
    private bool IsNewDirectionAvailable(Vector2Int[] directionOptions) => _lastDirectionOptions is null || directionOptions.Except(_lastDirectionOptions).Any();
    private IEnumerable<Vector2Int> GetNeighboursOf(Vector2Int position)
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
}
