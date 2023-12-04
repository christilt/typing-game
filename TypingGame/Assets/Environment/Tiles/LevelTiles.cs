using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UIElements;
using System;

public class LevelTiles : Singleton<LevelTiles>
{
    [SerializeField] private Tilemap _pathTiles;
    [SerializeField] private GameObject _keyIconPrefab;
    [SerializeField] private int _tilesInstantiatedPerFrame;

    private IReadOnlyDictionary<Vector2Int, KeyTile> _keyTilesByPosition;

    public Tilemap PathTiles => _pathTiles;

    private void Start()
    {
        GameManager.Instance.OnStateChanging += OnGameStateChanging;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanging -= OnGameStateChanging;
    }

    public event Action OnInitialised;

    private void OnGameStateChanging(GameState state)
    {
        if (state.EndsPlayerControl())
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
            var uniqueKeyBounds = new BoundsInt(
                xMin: position.x - 2,
                yMin: position.y - 2,
                zMin: 0,
                sizeX: 5,
                sizeY: 5,
                sizeZ: 1);

            var keysThatWouldBeDuplicates = new List<char>();
            foreach(var uniqueKeyBoundsPosition in uniqueKeyBounds.allPositionsWithin)
            {
                if (keyTilesByPosition.TryGetValue((Vector2Int) uniqueKeyBoundsPosition, out KeyTile keyTile))
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
}