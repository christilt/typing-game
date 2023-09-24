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

    private float _secondsSinceLastMove;
    private float _secondsBetweenMoves;

    private Vector2Int[] _lastDirectionOptions;
    private Vector2Int _lastDirection = Vector2Int.up;
    private Vector2Int _direction = Vector2Int.up;

    private Vector3Int _cellPosition3;
    private Vector2Int _cellPosition2;

    private Vector2Int LastDirectionOpposite => _lastDirection * -1;

    void Awake()
    {
        _secondsBetweenMoves = 1f / _movesPerSecond;
        _allowedPositions = new HashSet<Vector2Int>(_allowedTiles.GetPositions());
    }

    private void Update()
    {
        _cellPosition3 = _allowedTiles.WorldToCell(transform.position);
        _cellPosition2 = (Vector2Int)_cellPosition3;
        _secondsSinceLastMove += Time.deltaTime;
        if (_secondsSinceLastMove > _secondsBetweenMoves)
        {
            UpdateDirections();
            transform.position += new Vector3(_direction.x, _direction.y);
            _secondsSinceLastMove = 0;
        }
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
        return GetNeighboursOf(_cellPosition2)
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

    private Vector2Int MapPositionToDirection(Vector2Int position) => position - _cellPosition2;

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
