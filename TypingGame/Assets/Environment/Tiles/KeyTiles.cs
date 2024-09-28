using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UIElements;
using System;

public class KeyTiles : Singleton<KeyTiles>
{
    [SerializeField] private Tilemap _pathTiles;
    [SerializeField] private GameObject _keyIconPrefab;
    [SerializeField] private int _tilesInstantiatedPerFrame;

    private IReadOnlyDictionary<Vector2Int, KeyTile> _keyTilesByPosition;

    public Tilemap PathTiles => _pathTiles;

    private void Start()
    {
        GameplayManager.Instance.OnStateChanging += OnGameStateChanging;
    }

    private void OnDestroy()
    {
        if (GameplayManager.Instance != null)
            GameplayManager.Instance.OnStateChanging -= OnGameStateChanging;
    }

    public event Action OnInitialised;

    private void OnGameStateChanging(GameState state)
    {
        if (state.EndsGameplay())
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public IReadOnlyCollection<Vector2Int> Positions { get; private set; }

    public void Initialise(Action onComplete = null)
    {
        StartCoroutine(InstantiateKeyTilesByPosition(onComplete));
    }

    public Vector3 WorldPositionCentre(Vector3Int position) => _pathTiles.GetCellCenterWorld(position);

    public IEnumerable<KeyTile> GetNeighboursOf(Vector3 position) => GetNeighboursOf((Vector2Int)_pathTiles.WorldToCell(position));

    public IEnumerable<KeyTile> GetNeighboursOf(Vector2Int position)
    {
        if (_keyTilesByPosition.TryGetValue(position + Vector2Int.up, out KeyTile upTile))
            yield return upTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector2Int.down, out KeyTile downTile))
            yield return downTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector2Int.left, out KeyTile leftTile))
            yield return leftTile;
        if (_keyTilesByPosition.TryGetValue(position + Vector2Int.right, out KeyTile rightTile))
            yield return rightTile;
    }

    public List<KeyTile> GetRandomTiles(int count)
    {
        count = Math.Min(count, _keyTilesByPosition.Count);

        var chosenIndicies = new List<int>();

        for (var i = 0; i < count; i++)
        {
            var candidateIndex = UnityEngine.Random.Range(0, _keyTilesByPosition.Count);
            if (chosenIndicies.Contains(candidateIndex))
                continue;

            chosenIndicies.Add(candidateIndex);
        }

        return chosenIndicies
            .Select(i => _keyTilesByPosition.ElementAt(i).Value)
            .ToList();
    }

    // Naive approach to ensuring uniqueness - could be more efficient
    private IEnumerator InstantiateKeyTilesByPosition(Action onComplete = null)
    {
        var positions = _pathTiles.GetPositions();
        var keyTilesByPosition = new Dictionary<Vector2Int, KeyTile>();
        foreach (var position in positions)
        {
            var keysThatWouldBeDuplicates = new List<char>();
            foreach (var uniqueKeyBoundsPosition in Preceding2StepPositions(position))
            {
                if (keyTilesByPosition.TryGetValue((Vector2Int)uniqueKeyBoundsPosition, out KeyTile keyTile))
                    keysThatWouldBeDuplicates.Add(keyTile.Key);
            }

            var positionInt = (Vector3Int)position;
            var centre = WorldPositionCentre(positionInt);
            keyTilesByPosition.Add(
                position,
                KeyTile.Instantiate(
                    _keyIconPrefab,
                    positionInt, 
                    centre,
                    this.gameObject, 
                    deniedKeys: keysThatWouldBeDuplicates));

            if (keyTilesByPosition.Count % _tilesInstantiatedPerFrame == 0)
                yield return new WaitForEndOfFrame();
        }

        _keyTilesByPosition = keyTilesByPosition;
        Positions = _keyTilesByPosition.Keys.ToList();

        OnInitialised?.Invoke();

        if (onComplete != null)
            onComplete();
    }

    /// <summary>
    /// The positions "less than" the given position, that reach the given position in 2 vertical and or horizontal steps.
    /// "less than" is based on ordering used by tilemap.cellBounds.allPositionsWithin.
    /// </summary>
    private IEnumerable<Vector2Int> Preceding2StepPositions(Vector2Int position)
    {
        // 2 1 0
        // 3 2 1 2 3
        // 4 3 2 3 4

        yield return new Vector2Int(position.x, position.y - 2);
        yield return new Vector2Int(position.x - 1, position.y - 1);
        yield return new Vector2Int(position.x + 1, position.y - 1);
        yield return new Vector2Int(position.x - 2, position.y);
    }
}